namespace StockTradePro.BlazorUI.Models.Watchlist
{
    public class StockPriceDto
    {
        public string Symbol { get; set; } = string.Empty;
        public double Price { get; set; }
        public double Change { get; set; }
        public double ChangePercent { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    
}
