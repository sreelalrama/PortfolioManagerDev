using StockTradePro.BlazorUI.Models.Auth;

namespace StockTradePro.BlazorUI.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(UserLoginDto loginDto);
        Task<AuthResponseDto?> RegisterAsync(UserRegistrationDto registrationDto);
        Task<AuthResponseDto?> RefreshTokenAsync(RefreshTokenDto refreshTokenDto);
        Task<bool> LogoutAsync();
        Task<bool> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto);
        Task<UserDto?> GetCurrentUserAsync();
        Task<UserDto?> GetUserAsync(string userId);
        Task<bool> UpdateProfileAsync(UserDto userDto);
        Task<bool> IsAuthenticatedAsync();
        Task<string?> GetCurrentUserIdAsync();
        Task<string?> GetTokenAsync();
        void SetToken(string token);
        void ClearToken();
    }
}
