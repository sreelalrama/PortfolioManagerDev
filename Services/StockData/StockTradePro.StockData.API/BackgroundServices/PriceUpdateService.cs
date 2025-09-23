// ===== BackgroundServices/PriceUpdateService.cs - FIXED =====
using Microsoft.AspNetCore.SignalR;
using StockTradePro.StockData.API.Hubs;
using StockTradePro.StockData.API.Services;

namespace StockTradePro.StockData.API.BackgroundServices
{
    public class PriceUpdateService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IHubContext<StockPriceHub, IStockPriceClient> _hubContext;
        private readonly ILogger<PriceUpdateService> _logger;
        private readonly IConfiguration _configuration;
        private readonly TimeSpan _updateInterval;
        private readonly bool _enableRealTimeUpdates;

        public PriceUpdateService(
            IServiceScopeFactory serviceScopeFactory,
            IHubContext<StockPriceHub, IStockPriceClient> hubContext,
            ILogger<PriceUpdateService> logger,
            IConfiguration configuration)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _hubContext = hubContext;
            _logger = logger;
            _configuration = configuration;

            var intervalSeconds = _configuration.GetValue<int>("StockDataSettings:PriceUpdateIntervalSeconds", 30);
            _updateInterval = TimeSpan.FromSeconds(intervalSeconds);
            _enableRealTimeUpdates = _configuration.GetValue<bool>("StockDataSettings:EnableRealTimeUpdates", true);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_enableRealTimeUpdates)
            {
                _logger.LogInformation("Real-time price updates are disabled");
                return;
            }

            _logger.LogInformation("Price Update Service started with {Interval} second intervals",
                _updateInterval.TotalSeconds);

            // Wait a bit for application to fully start
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var priceSimulationService = scope.ServiceProvider.GetRequiredService<IPriceSimulationService>();
                    var stockService = scope.ServiceProvider.GetRequiredService<IStockService>();

                    var isMarketOpen = await priceSimulationService.IsMarketOpenAsync();

                    if (isMarketOpen)
                    {
                        await UpdateStockPricesAsync(priceSimulationService, stockService);
                    }
                    else
                    {
                        // During market closed hours, update less frequently and with smaller movements
                        await UpdateStockPricesSlowAsync(priceSimulationService, stockService);
                    }

                    // Send market status update every 5 minutes
                    if (DateTime.UtcNow.Minute % 5 == 0)
                    {
                        await SendMarketStatusUpdate(priceSimulationService);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in price update cycle");
                }

                // Wait for next update cycle
                try
                {
                    await Task.Delay(_updateInterval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // Expected when cancellation is requested
                    break;
                }
            }

            _logger.LogInformation("Price Update Service stopped");
        }

        private async Task UpdateStockPricesAsync(IPriceSimulationService priceSimulationService, IStockService stockService)
        {
            try
            {
                var startTime = DateTime.UtcNow;

                // Update all stock prices
                await priceSimulationService.UpdateAllStockPricesAsync();

                // Get recently updated stocks
                var updatedStocks = await priceSimulationService.GetUpdatedStocksAsync(startTime.AddSeconds(-5));

                if (updatedStocks.Any())
                {
                    var isMarketOpen = await priceSimulationService.IsMarketOpenAsync();

                    // Create price update DTOs
                    var priceUpdates = updatedStocks.Select(stock => new StockPriceUpdateDto
                    {
                        Symbol = stock.Symbol,
                        CompanyName = stock.CompanyName,
                        CurrentPrice = stock.CurrentPrice,
                        PriceChange = stock.PriceChange,
                        PriceChangePercent = stock.PriceChangePercent,
                        Volume = stock.Volume,
                        HighPrice = GetHighPrice(stock),
                        LowPrice = GetLowPrice(stock),
                        LastUpdated = stock.LastUpdated,
                        IsMarketOpen = isMarketOpen
                    }).ToList();

                    // Send bulk update to all subscribers
                    await _hubContext.Clients.Group("all_stocks").BulkPriceUpdate(priceUpdates);

                    // Send individual updates to specific stock subscribers
                    foreach (var update in priceUpdates)
                    {
                        await _hubContext.Clients.Group($"stock_{update.Symbol}")
                            .PriceUpdate(update);
                    }

                    _logger.LogDebug("Sent price updates for {Count} stocks during market hours", priceUpdates.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock prices during market hours");
            }
        }

        private async Task UpdateStockPricesSlowAsync(IPriceSimulationService priceSimulationService, IStockService stockService)
        {
            try
            {
                // During market closed, update only a few stocks to simulate after-hours trading
                var randomStocks = new[] { "AAPL", "MSFT", "GOOGL", "AMZN", "TSLA", "META", "NVDA" };
                var random = new Random();
                var selectedStock = randomStocks[random.Next(randomStocks.Length)];

                var updatedPrice = await priceSimulationService.UpdateStockPriceAsync(selectedStock);

                if (updatedPrice != null)
                {
                    var stock = await stockService.GetStockBySymbolAsync(selectedStock);
                    if (stock != null)
                    {
                        var priceUpdate = new StockPriceUpdateDto
                        {
                            Symbol = stock.Symbol,
                            CompanyName = stock.CompanyName,
                            CurrentPrice = stock.CurrentPrice,
                            PriceChange = stock.PriceChange,
                            PriceChangePercent = stock.PriceChangePercent,
                            Volume = stock.Volume,
                            HighPrice = GetHighPrice(stock),
                            LowPrice = GetLowPrice(stock),
                            LastUpdated = stock.LastUpdated,
                            IsMarketOpen = false
                        };

                        // Send update to subscribers
                        await _hubContext.Clients.Group("all_stocks").PriceUpdate(priceUpdate);
                        await _hubContext.Clients.Group($"stock_{priceUpdate.Symbol}").PriceUpdate(priceUpdate);

                        _logger.LogDebug("Sent after-hours price update for {Symbol}", selectedStock);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock prices during market closed hours");
            }
        }

        private async Task SendMarketStatusUpdate(IPriceSimulationService priceSimulationService)
        {
            try
            {
                var isMarketOpen = await priceSimulationService.IsMarketOpenAsync();

                var marketStatus = new
                {
                    IsOpen = isMarketOpen,
                    TimeUntilOpen = isMarketOpen ? TimeSpan.Zero : priceSimulationService.GetTimeUntilMarketOpen(),
                    TimeUntilClose = isMarketOpen ? priceSimulationService.GetTimeUntilMarketClose() : TimeSpan.Zero,
                    Timestamp = DateTime.UtcNow,
                    NextUpdate = DateTime.UtcNow.Add(_updateInterval)
                };

                // FIXED: Use the strongly-typed interface method instead of SendAsync
                await _hubContext.Clients.All.MarketStatus(marketStatus);

                _logger.LogDebug("Sent market status update - Market is {Status}",
                    isMarketOpen ? "OPEN" : "CLOSED");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending market status update");
            }
        }

        private static decimal GetHighPrice(Models.DTOs.StockDto stock)
        {
            // For simplicity, calculate high price as current price + 1%
            // In a real implementation, this would come from the actual StockPrice entity
            return stock.CurrentPrice * 1.01m;
        }

        private static decimal GetLowPrice(Models.DTOs.StockDto stock)
        {
            // For simplicity, calculate low price as current price - 1%
            // In a real implementation, this would come from the actual StockPrice entity
            return stock.CurrentPrice * 0.99m;
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Price Update Service is stopping...");

            try
            {
                // FIXED: Use the strongly-typed interface method instead of SendAsync
                var shutdownMessage = "Price update service is shutting down";
                await _hubContext.Clients.All.Error(shutdownMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error notifying clients of service shutdown");
            }

            await base.StopAsync(stoppingToken);

            _logger.LogInformation("Price Update Service stopped successfully");
        }
    }
}