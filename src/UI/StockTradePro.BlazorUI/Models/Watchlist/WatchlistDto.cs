namespace StockTradePro.BlazorUI.Models.Watchlist
{
    public class WatchlistDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<WatchlistItemDto> Items { get; set; } = new();
        public List<PriceAlertDto> PriceAlerts { get; set; } = new();
    }
}
