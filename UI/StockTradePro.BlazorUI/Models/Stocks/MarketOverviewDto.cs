namespace StockTradePro.BlazorUI.Models.Stocks
{
    public class MarketOverviewDto
    {
        public List<MarketIndexDto> Indices { get; set; } = new();
        public List<StockDto> TopGainers { get; set; } = new();
        public List<StockDto> TopLosers { get; set; } = new();
        public List<StockDto> MostActive { get; set; } = new();
        public DateTime LastUpdated { get; set; }
    }
}
