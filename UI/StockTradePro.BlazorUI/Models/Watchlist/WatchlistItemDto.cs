using StockTradePro.BlazorUI.Models.Stocks;

namespace StockTradePro.BlazorUI.Models.Watchlist
{
    public class WatchlistItemDto
    {
        public int Id { get; set; }
        public int WatchlistId { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public DateTime AddedAt { get; set; }
        public StockPriceDto? CurrentPrice { get; set; }
    }
}
