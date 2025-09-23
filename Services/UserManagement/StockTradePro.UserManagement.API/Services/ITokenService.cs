using StockTradePro.UserManagement.API.Models;

namespace StockTradePro.UserManagement.API.Services
{
    public interface ITokenService
    {
        Task<string> GenerateJwtTokenAsync(ApplicationUser user);
        Task<string> GenerateRefreshTokenAsync();
        Task<bool> ValidateRefreshTokenAsync(string userId, string refreshToken);
        Task SaveRefreshTokenAsync(string userId, string refreshToken);
        Task RevokeRefreshTokenAsync(string userId);
    }
}
