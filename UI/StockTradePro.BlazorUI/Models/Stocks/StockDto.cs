namespace StockTradePro.BlazorUI.Models.Stocks
{
    public class StockDto
    {
        public int Id { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string Exchange { get; set; } = string.Empty;
        public string Sector { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
        public double MarketCap { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public double CurrentPrice { get; set; }
        public double PriceChange { get; set; }
        public double PriceChangePercent { get; set; }
        public long Volume { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
