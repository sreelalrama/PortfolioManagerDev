using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockTradePro.NotificationService.API.Models
{

    public enum NotificationType
{
    PriceAlert,
    PortfolioUpdate,
    SystemAnnouncement,
    MarketNews,
    WatchlistUpdate
}

public enum NotificationStatus
{
    Pending,
    Sent,
    Failed,
    Read
}

public enum DeliveryMethod
{
    InApp,
    Email,
    Push
}

public class Notification
{
    [Key]
    public int Id { get; set; }

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

    public string? Data { get; set; } // JSON data for additional context

    public NotificationStatus Status { get; set; } = NotificationStatus.Pending;

    public DeliveryMethod Method { get; set; } = DeliveryMethod.InApp;

    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime? ReadAt { get; set; }

    public int? RetryCount { get; set; } = 0;
    public string? ErrorMessage { get; set; }
}
}