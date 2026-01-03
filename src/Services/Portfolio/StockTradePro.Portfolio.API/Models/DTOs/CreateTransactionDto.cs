using System.ComponentModel.DataAnnotations;

namespace StockTradePro.Portfolio.API.Models.DTOs
{
    public class CreateTransactionDto
    {
        [Required]
        public int PortfolioId { get; set; }

        [Required]
        [MaxLength(10)]
        public string Symbol { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(BUY|SELL)$")]
        public string Type { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Fees { get; set; } = 0;

        [MaxLength(500)]
        public string? Notes { get; set; }

        public DateTime? TransactionDate { get; set; }
    }
}
