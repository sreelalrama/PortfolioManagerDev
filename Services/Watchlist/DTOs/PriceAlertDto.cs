using System.ComponentModel.DataAnnotations;
using StockTradePro.WatchlistService.API.Models;

namespace StockTradePro.WatchlistService.API.DTOs
{
    public class PriceAlertDto
    {
        public int Id { get; set; }
        public int WatchlistId { get; set; }
        public string Symbol { get; set; }
        public AlertType Type { get; set; }
        public decimal TargetValue { get; set; }
        public AlertStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? TriggeredAt { get; set; }
        public string Notes { get; set; }
    }

    public class CreatePriceAlertDto
    {
        [Required]
        [StringLength(10)]
        public string Symbol { get; set; }

        [Required]
        public AlertType Type { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal TargetValue { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }
    }
}