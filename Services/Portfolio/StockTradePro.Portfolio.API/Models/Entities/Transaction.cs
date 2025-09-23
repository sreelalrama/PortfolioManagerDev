using System.ComponentModel.DataAnnotations;

namespace StockTradePro.Portfolio.API.Models.Entities
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PortfolioId { get; set; }

        [Required]
        [MaxLength(10)]
        public string Symbol { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Type { get; set; } = string.Empty; // BUY, SELL

        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Fees { get; set; } = 0;
        public decimal NetAmount { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Portfolio Portfolio { get; set; } = null!;
    }
}