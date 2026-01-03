
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockTradePro.WatchlistService.API.Models
{
    public class Watchlist
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public string UserId { get; set; }

        public bool IsDefault { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<WatchlistItem> Items { get; set; } = new List<WatchlistItem>();
        public virtual ICollection<PriceAlert> PriceAlerts { get; set; } = new List<PriceAlert>();
    }
}