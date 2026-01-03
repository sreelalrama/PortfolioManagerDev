
using System.ComponentModel.DataAnnotations;
using StockTradePro.NotificationService.API.Models;

namespace StockTradePro.NotificationService.API.DTOs
{
    public class CreateNotificationDto
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public NotificationType Type { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(1000)]
        public string Message { get; set; }

        public string? Data { get; set; }

        public DeliveryMethod Method { get; set; } = DeliveryMethod.InApp;
    }
}