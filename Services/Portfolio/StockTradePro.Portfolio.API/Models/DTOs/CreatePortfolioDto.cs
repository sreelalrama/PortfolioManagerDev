using System.ComponentModel.DataAnnotations;

namespace StockTradePro.Portfolio.API.Models.DTOs
{
    public class CreatePortfolioDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = "General";

        public decimal InitialValue { get; set; } = 0;
        public bool IsPublic { get; set; } = false;
    }
}