
using StockTradePro.NotificationService.API.DTOs;

namespace StockTradePro.NotificationService.API.Services
{
    public interface INotificationService
    {
        // Notification management
        Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto dto);
        Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(string userId, int page = 1, int pageSize = 20);
        Task<NotificationDto?> GetNotificationAsync(int id, string userId);
        Task<bool> MarkAsReadAsync(int id, string userId);
        Task<bool> MarkAllAsReadAsync(string userId);
        Task<int> GetUnreadCountAsync(string userId);
        Task<bool> DeleteNotificationAsync(int id, string userId);

        // Specialized notification creators
        Task SendPriceAlertNotificationAsync(PriceAlertRequestDto request);
        Task SendPortfolioUpdateNotificationAsync(PortfolioUpdateRequestDto request);
        Task SendSystemAnnouncementAsync(string title, string message, IEnumerable<string>? userIds = null);

        // Real-time notifications
        Task SendRealTimeNotificationAsync(string userId, NotificationDto notification);
        Task SendRealTimeNotificationToAllAsync(NotificationDto notification);
    }
}
