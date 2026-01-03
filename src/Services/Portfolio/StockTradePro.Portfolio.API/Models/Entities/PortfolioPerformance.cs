using System.ComponentModel.DataAnnotations;

namespace StockTradePro.Portfolio.API.Models.Entities
{
    public class PortfolioPerformance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PortfolioId { get; set; }

        public DateTime Date { get; set; }
        public decimal TotalValue { get; set; }
        public decimal DayChange { get; set; }
        public decimal DayChangePercent { get; set; }
        public decimal TotalReturn { get; set; }
        public decimal TotalReturnPercent { get; set; }

        // Performance metrics
        public int TotalHoldings { get; set; }
        public decimal CashValue { get; set; } = 0;
        public decimal MarketValue { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Portfolio Portfolio { get; set; } = null!;
    }
}