namespace StockTradePro.Portfolio.API.Models.DTOs
{
    public class PortfolioSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal CurrentValue { get; set; }
        public decimal TotalGainLoss { get; set; }
        public decimal TotalGainLossPercent { get; set; }
        public int TotalHoldings { get; set; }
        public DateTime LastCalculatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}