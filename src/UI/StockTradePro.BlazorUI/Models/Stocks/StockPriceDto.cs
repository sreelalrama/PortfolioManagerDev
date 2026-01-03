namespace StockTradePro.BlazorUI.Models.Stocks
{
    public class StockPriceDto
    {
        public int Id { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public double OpenPrice { get; set; }
        public double HighPrice { get; set; }
        public double LowPrice { get; set; }
        public double ClosePrice { get; set; }
        public double CurrentPrice { get; set; }
        public long Volume { get; set; }
        public double PriceChange { get; set; }
        public double PriceChangePercent { get; set; }
        public DateTime Date { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
