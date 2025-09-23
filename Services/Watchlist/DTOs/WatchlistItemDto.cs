
using System.ComponentModel.DataAnnotations;

namespace StockTradePro.WatchlistService.API.DTOs
{
    public class WatchlistItemDto
    {
        public int Id { get; set; }
        public int WatchlistId { get; set; }
        public string Symbol { get; set; }
        public int SortOrder { get; set; }
        public DateTime AddedAt { get; set; }
        public StockPriceDto? CurrentPrice { get; set; }
    }

    public class AddWatchlistItemDto
    {
        [Required]
        [StringLength(10)]
        public string Symbol { get; set; }

        public int SortOrder { get; set; }
    }

    public class StockPriceDto
    {
        public string Symbol { get; set; }
        public decimal Price { get; set; }
        public decimal Change { get; set; }
        public decimal ChangePercent { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}