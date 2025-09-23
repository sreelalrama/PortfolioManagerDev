using System.ComponentModel.DataAnnotations;

namespace StockTradePro.StockData.API.Models
{
    public class Stock
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(10)]
        public string Symbol { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string CompanyName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Exchange { get; set; } = string.Empty; // NASDAQ, NYSE, etc.

        [MaxLength(100)]
        public string Sector { get; set; } = string.Empty; // Technology, Healthcare, etc.

        [MaxLength(100)]
        public string Industry { get; set; } = string.Empty; // Software, Pharmaceuticals, etc.

        public decimal MarketCap { get; set; } // Market capitalization

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(200)]
        public string? Website { get; set; }

        [MaxLength(200)]
        public string? LogoUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<StockPrice> StockPrices { get; set; } = new List<StockPrice>();
    }
}
