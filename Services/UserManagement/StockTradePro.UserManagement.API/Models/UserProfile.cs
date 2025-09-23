using System.ComponentModel.DataAnnotations;

namespace StockTradePro.UserManagement.API.Models
{
    public class UserProfile
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Occupation { get; set; }

        [MaxLength(100)]
        public string? Company { get; set; }

        [MaxLength(200)]
        public string? Location { get; set; }

        public decimal? AnnualIncome { get; set; }

        [MaxLength(50)]
        public string? RiskTolerance { get; set; } // Conservative, Moderate, Aggressive

        [MaxLength(50)]
        public string? InvestmentExperience { get; set; } // Beginner, Intermediate, Advanced

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public virtual ApplicationUser User { get; set; } = null!;
    }
}
