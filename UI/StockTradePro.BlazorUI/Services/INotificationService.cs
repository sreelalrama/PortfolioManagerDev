using StockTradePro.BlazorUI.Models.Notifications;

namespace StockTradePro.BlazorUI.Services
{
    public interface INotificationService
    {
        Task<List<NotificationDto>> GetNotificationsAsync(int page = 1, int pageSize = 20);
        Task<NotificationDto?> GetNotificationAsync(int id);
        Task<NotificationDto?> CreateNotificationAsync(CreateNotificationDto createDto);
        Task<bool> DeleteNotificationAsync(int id);
        Task<bool> MarkNotificationReadAsync(int id);
        Task<bool> MarkAllNotificationsReadAsync();
        Task<int> GetUnreadCountAsync();
        Task<List<NotificationPreferenceDto>> GetNotificationPreferencesAsync();
        Task<NotificationPreferenceDto?> GetNotificationPreferenceAsync(NotificationType type);
        Task<NotificationPreferenceDto?> UpdateNotificationPreferenceAsync(NotificationType type, UpdateNotificationPreferenceDto updateDto);
        Task<bool> InitializePreferencesAsync();
    }
}
