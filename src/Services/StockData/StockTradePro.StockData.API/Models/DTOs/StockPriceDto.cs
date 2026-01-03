
namespace StockTradePro.StockData.API.Models.DTOs
{

    public class StockPriceDto
    {
        public int Id { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public decimal OpenPrice { get; set; }
        public decimal HighPrice { get; set; }
        public decimal LowPrice { get; set; }
        public decimal ClosePrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public long Volume { get; set; }
        public decimal PriceChange { get; set; }
        public decimal PriceChangePercent { get; set; }
        public DateTime Date { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}