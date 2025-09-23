using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace StockTradePro.UserManagement.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(100)]
        public string? FirstName { get; set; }

        [MaxLength(100)]
        public string? LastName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLoginAt { get; set; }

        public bool IsActive { get; set; } = true;

        [MaxLength(200)]
        public string? ProfileImageUrl { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(500)]
        public string? Bio { get; set; }

        // Navigation properties
        public virtual ICollection<UserProfile> Profiles { get; set; } = new List<UserProfile>();
    }
}