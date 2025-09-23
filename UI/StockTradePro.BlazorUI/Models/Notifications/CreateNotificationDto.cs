namespace StockTradePro.BlazorUI.Models.Notifications
{
    public class CreateNotificationDto
    {
        public string UserId { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
        public DeliveryMethod Method { get; set; }
    }
}
