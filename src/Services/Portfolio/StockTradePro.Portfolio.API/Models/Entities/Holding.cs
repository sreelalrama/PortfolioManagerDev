using System.ComponentModel.DataAnnotations;

namespace StockTradePro.Portfolio.API.Models.Entities
{
    public class Holding
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PortfolioId { get; set; }

        [Required]
        [MaxLength(10)]
        public string Symbol { get; set; } = string.Empty;

        [MaxLength(200)]
        public string CompanyName { get; set; } = string.Empty;

        public int Quantity { get; set; }
        public decimal AverageCost { get; set; }
        public decimal TotalCost { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal UnrealizedGainLoss { get; set; }
        public decimal UnrealizedGainLossPercent { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Portfolio Portfolio { get; set; } = null!;
    }
}