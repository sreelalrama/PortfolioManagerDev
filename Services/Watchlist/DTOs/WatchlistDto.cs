using System.ComponentModel.DataAnnotations;

namespace StockTradePro.WatchlistService.API.DTOs
{
    public class WatchlistDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public bool IsDefault { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<WatchlistItemDto> Items { get; set; } = new();
        public List<PriceAlertDto> PriceAlerts { get; set; } = new();
    }

    public class CreateWatchlistDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public bool IsDefault { get; set; }
    }

    public class UpdateWatchlistDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }
    }
}