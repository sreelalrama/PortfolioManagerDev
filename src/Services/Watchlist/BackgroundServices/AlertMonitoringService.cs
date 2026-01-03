using Microsoft.Extensions.DependencyInjection;
using StockTradePro.WatchlistService.API.Models;
using StockTradePro.WatchlistService.API.Services;
using StockTradePro.Shared.Messages;

namespace StockTradePro.WatchlistService.API.BackgroundServices
{
    public class AlertMonitoringService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AlertMonitoringService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(30);

        public AlertMonitoringService(IServiceProvider serviceProvider, ILogger<AlertMonitoringService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Alert Monitoring Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckPriceAlerts();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while checking price alerts");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Alert Monitoring Service stopped");
        }

        private async Task CheckPriceAlerts()
        {
            using var scope = _serviceProvider.CreateScope();
            var priceAlertService = scope.ServiceProvider.GetRequiredService<IPriceAlertService>();
            var stockDataService = scope.ServiceProvider.GetRequiredService<IStockDataService>();
            var messagePublisher = scope.ServiceProvider.GetRequiredService<IMessagePublisher>();
            var watchlistService = scope.ServiceProvider.GetRequiredService<IWatchlistService>();

            var activeAlerts = await priceAlertService.GetActiveAlertsAsync();

            foreach (var alert in activeAlerts)
            {
                try
                {
                    var currentPrice = await stockDataService.GetCurrentPriceAsync(alert.Symbol);
                    if (currentPrice == null) continue;

                    bool shouldTrigger = alert.Type switch
                    {
                        AlertType.Above => currentPrice.Price >= alert.TargetValue,
                        AlertType.Below => currentPrice.Price <= alert.TargetValue,
                        AlertType.PercentageGain => currentPrice.ChangePercent >= alert.TargetValue,
                        AlertType.PercentageLoss => currentPrice.ChangePercent <= -alert.TargetValue,
                        _ => false
                    };

                    if (shouldTrigger)
                    {
                        // Update alert status
                        await priceAlertService.TriggerAlertAsync(alert.Id);

                        // Get user ID from watchlist
                        var watchlist = await watchlistService.GetWatchlistAsync(alert.WatchlistId, "system");
                        if (watchlist == null) continue;

                        // Publish message to RabbitMQ
                        var message = new PriceAlertMessage
                        {
                            UserId = watchlist.UserId,
                            AlertId = alert.Id,
                            Symbol = alert.Symbol,
                            AlertType = alert.Type.ToString(),
                            TargetValue = alert.TargetValue,
                            CurrentPrice = currentPrice.Price,
                            ChangePercent = currentPrice.ChangePercent,
                            TriggeredAt = DateTime.UtcNow,
                            Notes = alert.Notes
                        };

                        await messagePublisher.PublishPriceAlertAsync(message);

                        _logger.LogInformation("Price alert {AlertId} triggered for {Symbol} at {Price}",
                            alert.Id, alert.Symbol, currentPrice.Price);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error checking alert {AlertId} for symbol {Symbol}", alert.Id, alert.Symbol);
                }
            }
        }
    }
}