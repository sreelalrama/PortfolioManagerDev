namespace StockTradePro.StockData.API.Models.DTOs
{
    public class MarketIndexDto
    {
        public string IndexName { get; set; } = string.Empty;
        public string IndexSymbol { get; set; } = string.Empty;
        public decimal CurrentValue { get; set; }
        public decimal Change { get; set; }
        public decimal ChangePercent { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
