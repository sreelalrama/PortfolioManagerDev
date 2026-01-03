using StockTradePro.UserManagement.API.Models.DTOs;

namespace StockTradePro.UserManagement.API.Services
{
    public interface IUserService
    {
        Task<AuthResponseDto> RegisterAsync(UserRegistrationDto registrationDto);
        Task<AuthResponseDto> LoginAsync(UserLoginDto loginDto);
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken, string userId);
        Task<bool> LogoutAsync(string userId);
        Task<UserDto?> GetUserByIdAsync(string userId);
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<bool> UpdateUserAsync(string userId, UserDto userDto);
        Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
    }
}
