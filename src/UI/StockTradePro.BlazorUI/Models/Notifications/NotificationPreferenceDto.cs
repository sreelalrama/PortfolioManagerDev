namespace StockTradePro.BlazorUI.Models.Notifications
{
    public class NotificationPreferenceDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public bool InAppEnabled { get; set; }
        public bool EmailEnabled { get; set; }
        public bool PushEnabled { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
