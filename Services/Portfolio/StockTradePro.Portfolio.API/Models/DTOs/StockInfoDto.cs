namespace StockTradePro.Portfolio.API.Models.DTOs
{
    public class StockInfoDto
    {
        public string Symbol { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public decimal CurrentPrice { get; set; }
        public decimal PriceChange { get; set; }
        public decimal PriceChangePercent { get; set; }
        public long Volume { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}