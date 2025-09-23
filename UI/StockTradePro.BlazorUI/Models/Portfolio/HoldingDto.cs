namespace StockTradePro.BlazorUI.Models.Portfolio
{
    public class HoldingDto
    {
        public int Id { get; set; }
        public int PortfolioId { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public double AverageCost { get; set; }
        public double TotalCost { get; set; }
        public double CurrentPrice { get; set; }
        public double CurrentValue { get; set; }
        public double UnrealizedGainLoss { get; set; }
        public double UnrealizedGainLossPercent { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime CreatedAt { get; set; }
        public double PortfolioPercent { get; set; }
        public double DayChange { get; set; }
        public double DayChangePercent { get; set; }
    }
}
