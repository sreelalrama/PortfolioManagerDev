using System.ComponentModel.DataAnnotations;

namespace StockTradePro.UserManagement.API.Models.DTOs
{
    public class RefreshTokenDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;

        [Required]
        public string UserId { get; set; } = string.Empty;
    }
}
