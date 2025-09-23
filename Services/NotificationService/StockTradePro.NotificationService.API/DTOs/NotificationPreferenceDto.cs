
using StockTradePro.NotificationService.API.Models;

namespace StockTradePro.NotificationService.API.DTOs
{
    public class NotificationPreferenceDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public NotificationType Type { get; set; }
        public bool InAppEnabled { get; set; }
        public bool EmailEnabled { get; set; }
        public bool PushEnabled { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class UpdateNotificationPreferenceDto
    {
        public bool InAppEnabled { get; set; }
        public bool EmailEnabled { get; set; }
        public bool PushEnabled { get; set; }
    }

    public class BulkUpdatePreferencesDto
    {
        public Dictionary<NotificationType, UpdateNotificationPreferenceDto> Preferences { get; set; } = new();
    }
}