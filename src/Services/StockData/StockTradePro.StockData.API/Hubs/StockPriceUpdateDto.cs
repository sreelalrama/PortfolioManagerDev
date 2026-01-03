namespace StockTradePro.StockData.API.Hubs
{
    public class StockPriceUpdateDto
    {
        public string Symbol { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public decimal CurrentPrice { get; set; }
        public decimal PriceChange { get; set; }
        public decimal PriceChangePercent { get; set; }
        public long Volume { get; set; }
        public decimal HighPrice { get; set; }
        public decimal LowPrice { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsMarketOpen { get; set; }
    }
}