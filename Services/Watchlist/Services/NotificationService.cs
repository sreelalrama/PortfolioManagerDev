//using System.Text.Json;
//using StockTradePro.WatchlistService.API.DTOs;

//namespace StockTradePro.WatchlistService.API.Services
//{
//    public class NotificationService : INotificationService
//    {
//        private readonly HttpClient _httpClient;
//        private readonly IConfiguration _configuration;
//        private readonly ILogger<NotificationService> _logger;

//        public NotificationService(HttpClient httpClient, IConfiguration configuration, ILogger<NotificationService> logger)
//        {
//            _httpClient = httpClient;
//            _configuration = configuration;
//            _logger = logger;
//        }

//        public async Task SendPriceAlertNotificationAsync(PriceAlertDto alert, StockPriceDto currentPrice)
//        {
//            try
//            {
//                var notificationServiceUrl = _configuration["Services:NotificationService"];

//                var alertData = new
//                {
//                    UserId = "temp-user-id", // You'll need to get this from the alert/watchlist
//                    AlertData = new
//                    {
//                        AlertId = alert.Id,
//                        Symbol = alert.Symbol,
//                        AlertType = alert.Type.ToString(),
//                        TargetValue = alert.TargetValue,
//                        CurrentPrice = currentPrice.Price,
//                        ChangePercent = currentPrice.ChangePercent
//                    }
//                };

//                var json = JsonSerializer.Serialize(alertData);
//                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

//                var response = await _httpClient.PostAsync($"{notificationServiceUrl}/api/notifications/price-alert", content);

//                if (response.IsSuccessStatusCode)
//                {
//                    _logger.LogInformation("Price alert notification sent successfully for alert {AlertId}", alert.Id);
//                }
//                else
//                {
//                    _logger.LogWarning("Failed to send price alert notification. Status: {StatusCode}", response.StatusCode);
//                }
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error sending price alert notification for alert {AlertId}", alert.Id);
//            }
//        }

//        public async Task SendWatchlistUpdateNotificationAsync(string userId, string watchlistName, string message)
//        {
//            try
//            {
//                var notificationServiceUrl = _configuration["Services:NotificationService"];

//                var notificationData = new
//                {
//                    UserId = userId,
//                    Type = "WatchlistUpdate",
//                    Title = $"Watchlist Update: {watchlistName}",
//                    Message = message,
//                    Method = "InApp"
//                };

//                var json = JsonSerializer.Serialize(notificationData);
//                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

//                var response = await _httpClient.PostAsync($"{notificationServiceUrl}/api/notifications", content);

//                if (response.IsSuccessStatusCode)
//                {
//                    _logger.LogInformation("Watchlist update notification sent successfully to user {UserId}", userId);
//                }
//                else
//                {
//                    _logger.LogWarning("Failed to send watchlist update notification. Status: {StatusCode}", response.StatusCode);
//                }
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error sending watchlist update notification to user {UserId}", userId);
//            }
//        }
//    }
//}