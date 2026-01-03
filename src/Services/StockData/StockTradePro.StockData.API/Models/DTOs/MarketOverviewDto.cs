namespace StockTradePro.StockData.API.Models.DTOs
{
    public class MarketOverviewDto
    {
        public List<MarketIndexDto> Indices { get; set; } = new List<MarketIndexDto>();
        public List<StockDto> TopGainers { get; set; } = new List<StockDto>();
        public List<StockDto> TopLosers { get; set; } = new List<StockDto>();
        public List<StockDto> MostActive { get; set; } = new List<StockDto>();
        public DateTime LastUpdated { get; set; }
    }
}
