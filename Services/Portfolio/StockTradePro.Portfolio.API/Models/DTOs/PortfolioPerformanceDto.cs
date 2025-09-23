namespace StockTradePro.Portfolio.API.Models.DTOs
{
    public class PortfolioPerformanceDto
    {
        public int Id { get; set; }
        public int PortfolioId { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalValue { get; set; }
        public decimal DayChange { get; set; }
        public decimal DayChangePercent { get; set; }
        public decimal TotalReturn { get; set; }
        public decimal TotalReturnPercent { get; set; }
        public int TotalHoldings { get; set; }
        public decimal CashValue { get; set; }
        public decimal MarketValue { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}