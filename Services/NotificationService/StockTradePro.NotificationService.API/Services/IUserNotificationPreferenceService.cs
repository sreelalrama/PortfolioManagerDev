
using StockTradePro.NotificationService.API.DTOs;
using StockTradePro.NotificationService.API.Models;

namespace StockTradePro.NotificationService.API.Services
{
    public interface IUserNotificationPreferenceService
    {
        Task<IEnumerable<NotificationPreferenceDto>> GetUserPreferencesAsync(string userId);
        Task<NotificationPreferenceDto?> GetUserPreferenceAsync(string userId, NotificationType type);
        Task<NotificationPreferenceDto> UpdateUserPreferenceAsync(string userId, NotificationType type, UpdateNotificationPreferenceDto dto);
        Task<IEnumerable<NotificationPreferenceDto>> BulkUpdateUserPreferencesAsync(string userId, BulkUpdatePreferencesDto dto);
        Task CreateDefaultPreferencesForUserAsync(string userId);
    }
}