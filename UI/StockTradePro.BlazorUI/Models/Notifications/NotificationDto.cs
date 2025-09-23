namespace StockTradePro.BlazorUI.Models.Notifications
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
        public NotificationStatus Status { get; set; }
        public DeliveryMethod Method { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime? ReadAt { get; set; }
    }
}
