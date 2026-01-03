
using StockTradePro.NotificationService.API.Models;

namespace StockTradePro.NotificationService.API.DTOs
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public NotificationType Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string? Data { get; set; }
        public NotificationStatus Status { get; set; }
        public DeliveryMethod Method { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime? ReadAt { get; set; }
    }
}