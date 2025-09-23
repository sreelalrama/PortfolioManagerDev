namespace StockTradePro.BlazorUI.Models.Watchlist
{
    public class AddWatchlistItemDto
    {
        public string Symbol { get; set; } = string.Empty;
        public int SortOrder { get; set; }
    }
}
