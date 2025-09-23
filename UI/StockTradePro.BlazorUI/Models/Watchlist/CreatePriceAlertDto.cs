namespace StockTradePro.BlazorUI.Models.Watchlist
{
    public class CreatePriceAlertDto
    {
        public string Symbol { get; set; } = string.Empty;
        public AlertType Type { get; set; }
        public double TargetValue { get; set; }
        public string? Notes { get; set; }
    }

    public enum AlertType
    {
        Above = 0,
        Below = 1,
        PercentUp = 2,
        PercentDown = 3
    }

    public enum AlertStatus
    {
        Active = 0,
        Triggered = 1,
        Disabled = 2
    }
}
