using Microsoft.AspNetCore.SignalR;
using StockTradePro.StockData.API.Models.DTOs;
using StockTradePro.StockData.API.Services;

namespace StockTradePro.StockData.API.Hubs
{
    public class StockPriceHub : Hub<IStockPriceClient>
    {
        private readonly ILogger<StockPriceHub> _logger;
        private readonly IPriceSimulationService _priceSimulationService;

        public StockPriceHub(ILogger<StockPriceHub> logger, IPriceSimulationService priceSimulationService)
        {
            _logger = logger;
            _priceSimulationService = priceSimulationService;
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);

            // Send market status to new client
            var isMarketOpen = await _priceSimulationService.IsMarketOpenAsync();
            await Clients.Caller.MarketStatus(new
            {
                IsOpen = isMarketOpen,
                TimeUntilOpen = isMarketOpen ? TimeSpan.Zero : _priceSimulationService.GetTimeUntilMarketOpen(),
                TimeUntilClose = isMarketOpen ? _priceSimulationService.GetTimeUntilMarketClose() : TimeSpan.Zero,
                Timestamp = DateTime.UtcNow
            });

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Subscribe to updates for specific stocks
        /// </summary>
        public async Task SubscribeToStocks(List<string> symbols)
        {
            try
            {
                foreach (var symbol in symbols)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"stock_{symbol.ToUpper()}");
                }

                _logger.LogDebug("Client {ConnectionId} subscribed to {Count} stocks",
                    Context.ConnectionId, symbols.Count);

                await Clients.Caller.SubscriptionConfirmed(symbols);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error subscribing client {ConnectionId} to stocks", Context.ConnectionId);
                await Clients.Caller.Error("Failed to subscribe to stocks");
            }
        }

        /// <summary>
        /// Unsubscribe from specific stocks
        /// </summary>
        public async Task UnsubscribeFromStocks(List<string> symbols)
        {
            try
            {
                foreach (var symbol in symbols)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"stock_{symbol.ToUpper()}");
                }

                _logger.LogDebug("Client {ConnectionId} unsubscribed from {Count} stocks",
                    Context.ConnectionId, symbols.Count);

                await Clients.Caller.UnsubscriptionConfirmed(symbols);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unsubscribing client {ConnectionId} from stocks", Context.ConnectionId);
                await Clients.Caller.Error("Failed to unsubscribe from stocks");
            }
        }

        /// <summary>
        /// Subscribe to all stock updates
        /// </summary>
        public async Task SubscribeToAllStocks()
        {
            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "all_stocks");

                _logger.LogDebug("Client {ConnectionId} subscribed to all stocks", Context.ConnectionId);

                await Clients.Caller.AllStocksSubscriptionConfirmed();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error subscribing client {ConnectionId} to all stocks", Context.ConnectionId);
                await Clients.Caller.Error("Failed to subscribe to all stocks");
            }
        }

        /// <summary>
        /// Unsubscribe from all stock updates
        /// </summary>
        public async Task UnsubscribeFromAllStocks()
        {
            try
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, "all_stocks");

                _logger.LogDebug("Client {ConnectionId} unsubscribed from all stocks", Context.ConnectionId);

                await Clients.Caller.AllStocksUnsubscriptionConfirmed();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unsubscribing client {ConnectionId} from all stocks", Context.ConnectionId);
                await Clients.Caller.Error("Failed to unsubscribe from all stocks");
            }
        }

        /// <summary>
        /// Get current market status
        /// </summary>
        public async Task GetMarketStatus()
        {
            try
            {
                var isMarketOpen = await _priceSimulationService.IsMarketOpenAsync();

                await Clients.Caller.MarketStatus(new
                {
                    IsOpen = isMarketOpen,
                    TimeUntilOpen = isMarketOpen ? TimeSpan.Zero : _priceSimulationService.GetTimeUntilMarketOpen(),
                    TimeUntilClose = isMarketOpen ? _priceSimulationService.GetTimeUntilMarketClose() : TimeSpan.Zero,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting market status for client {ConnectionId}", Context.ConnectionId);
                await Clients.Caller.Error("Failed to get market status");
            }
        }
    }
}