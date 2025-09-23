namespace StockTradePro.BlazorUI.Models.Stocks
{
    public class MarketIndexDto
    {
        public string IndexName { get; set; } = string.Empty;
        public string IndexSymbol { get; set; } = string.Empty;
        public double CurrentValue { get; set; }
        public double Change { get; set; }
        public double ChangePercent { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
