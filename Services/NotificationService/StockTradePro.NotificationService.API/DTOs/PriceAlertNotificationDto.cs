
namespace StockTradePro.NotificationService.API.DTOs
{
    public class PriceAlertNotificationDto
    {
        public int AlertId { get; set; }
        public string Symbol { get; set; }
        public string AlertType { get; set; }
        public decimal TargetValue { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal ChangePercent { get; set; }
    }

    public class PriceAlertRequestDto
    {
        public string UserId { get; set; }
        public PriceAlertNotificationDto AlertData { get; set; }
    }
}