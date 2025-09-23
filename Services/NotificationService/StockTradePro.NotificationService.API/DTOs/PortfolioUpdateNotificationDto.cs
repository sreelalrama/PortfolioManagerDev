using System.ComponentModel.DataAnnotations;

namespace StockTradePro.NotificationService.API.DTOs
{
    public class PortfolioUpdateNotificationDto
    {
        public string PortfolioName { get; set; }
        public decimal TotalValue { get; set; }
        public decimal DayChange { get; set; }
        public decimal DayChangePercent { get; set; }
    }

    public class PortfolioUpdateRequestDto
    {
        public string UserId { get; set; }
        public PortfolioUpdateNotificationDto PortfolioData { get; set; }
    }

    public class SystemAnnouncementDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Message { get; set; }

        public IEnumerable<string>? UserIds { get; set; }
    }
}
