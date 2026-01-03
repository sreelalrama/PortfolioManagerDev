using System.ComponentModel.DataAnnotations;

namespace StockTradePro.Portfolio.API.Models.Entities
{
    public class Portfolio
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(450)] // Standard for user IDs
        public string UserId { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = "General"; // General, Retirement, Trading, etc.

        public decimal InitialValue { get; set; } = 0;
        public decimal CurrentValue { get; set; } = 0;
        public decimal TotalGainLoss { get; set; } = 0;
        public decimal TotalGainLossPercent { get; set; } = 0;

        public bool IsActive { get; set; } = true;
        public bool IsPublic { get; set; } = false; // For portfolio sharing

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastCalculatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public virtual ICollection<Holding> Holdings { get; set; } = new List<Holding>();
        public virtual ICollection<PortfolioPerformance> PerformanceHistory { get; set; } = new List<PortfolioPerformance>();
    }
}