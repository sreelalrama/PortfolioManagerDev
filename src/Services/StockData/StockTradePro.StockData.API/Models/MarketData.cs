using System.ComponentModel.DataAnnotations;

namespace StockTradePro.StockData.API.Models
{
    public class MarketData
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string IndexName { get; set; } = string.Empty; // S&P 500, NASDAQ, DOW, etc.

        [Required]
        [MaxLength(10)]
        public string IndexSymbol { get; set; } = string.Empty; // SPX, IXIC, DJI

        public decimal CurrentValue { get; set; }

        public decimal PreviousClose { get; set; }

        public decimal Change { get; set; }

        public decimal ChangePercent { get; set; }

        public long Volume { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
    }
}
