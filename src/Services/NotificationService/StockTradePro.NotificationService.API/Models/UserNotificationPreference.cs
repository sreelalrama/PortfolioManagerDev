using System.ComponentModel.DataAnnotations;

namespace StockTradePro.NotificationService.API.Models
{
    public class UserNotificationPreference
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public NotificationType Type { get; set; }

        public bool InAppEnabled { get; set; } = true;
        public bool EmailEnabled { get; set; } = true;
        public bool PushEnabled { get; set; } = false;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
