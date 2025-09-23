using System.ComponentModel.DataAnnotations;

namespace StockTradePro.StockData.API.Models
{
    public class StockPrice
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StockId { get; set; }

        public decimal OpenPrice { get; set; }

        public decimal HighPrice { get; set; }

        public decimal LowPrice { get; set; }

        public decimal ClosePrice { get; set; }

        public decimal CurrentPrice { get; set; } // Real-time price

        public long Volume { get; set; }

        public decimal PriceChange { get; set; } // Change from previous close

        public decimal PriceChangePercent { get; set; } // Percentage change

        public DateTime Date { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        public bool IsCurrentPrice { get; set; } = false; // True for latest price

        // Navigation property
        public virtual Stock Stock { get; set; } = null!;
    }
}
