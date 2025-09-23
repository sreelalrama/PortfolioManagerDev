namespace StockTradePro.BlazorUI.Models.Watchlist
{
    public class PriceAlertDto
    {
        public int Id { get; set; }
        public int WatchlistId { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public AlertType Type { get; set; }
        public double TargetValue { get; set; }
        public AlertStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? TriggeredAt { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
