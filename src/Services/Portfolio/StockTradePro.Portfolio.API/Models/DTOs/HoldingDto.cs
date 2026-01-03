namespace StockTradePro.Portfolio.API.Models.DTOs
{
    public class HoldingDto
    {
        public int Id { get; set; }
        public int PortfolioId { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal AverageCost { get; set; }
        public decimal TotalCost { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal UnrealizedGainLoss { get; set; }
        public decimal UnrealizedGainLossPercent { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime CreatedAt { get; set; }

        // Additional calculated fields
        public decimal PortfolioPercent { get; set; }
        public decimal DayChange { get; set; }
        public decimal DayChangePercent { get; set; }
    }
}