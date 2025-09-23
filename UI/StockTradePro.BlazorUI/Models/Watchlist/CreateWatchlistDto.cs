namespace StockTradePro.BlazorUI.Models.Watchlist
{
    public class CreateWatchlistDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
    }
}
