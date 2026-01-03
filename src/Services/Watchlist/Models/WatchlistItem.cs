using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockTradePro.WatchlistService.API.Models
{
    public class WatchlistItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int WatchlistId { get; set; }

        [Required]
        [StringLength(10)]
        public string Symbol { get; set; }

        public int SortOrder { get; set; }

        public DateTime AddedAt { get; set; }

        [ForeignKey("WatchlistId")]
        public virtual Watchlist Watchlist { get; set; }
    }
}
