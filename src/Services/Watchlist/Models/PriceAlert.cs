using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockTradePro.WatchlistService.API.Models
{
    public enum AlertType
    {
        Above,
        Below,
        PercentageGain,
        PercentageLoss
    }

    public enum AlertStatus
    {
        Active,
        Triggered,
        Disabled
    }

    public class PriceAlert
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int WatchlistId { get; set; }

        [Required]
        [StringLength(10)]
        public string Symbol { get; set; }

        [Required]
        public AlertType Type { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TargetValue { get; set; }

        public AlertStatus Status { get; set; } = AlertStatus.Active;

        public DateTime CreatedAt { get; set; }
        public DateTime? TriggeredAt { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        [ForeignKey("WatchlistId")]
        public virtual Watchlist Watchlist { get; set; }
    }
}
