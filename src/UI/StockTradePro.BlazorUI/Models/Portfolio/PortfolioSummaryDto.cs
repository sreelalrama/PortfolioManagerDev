namespace StockTradePro.BlazorUI.Models.Portfolio
{
    public class PortfolioSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public double CurrentValue { get; set; }
        public double TotalGainLoss { get; set; }
        public double TotalGainLossPercent { get; set; }
        public int TotalHoldings { get; set; }
        public DateTime LastCalculatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
