using StockTradePro.BlazorUI.Constants;
using StockTradePro.BlazorUI.Models.Auth;
using System.Text;

namespace StockTradePro.BlazorUI.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ILogger<AuthService> _logger;

        public AuthService(HttpClient httpClient, ILogger<AuthService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<AuthResponseDto?> LoginAsync(UserLoginDto loginDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(loginDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(ServiceConstants.ApiEndpoints.Login, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<AuthResponseDto>(responseJson, _jsonOptions);
                }
                else
                {
                    _logger.LogWarning("Login failed. Status: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
            }
            return null;
        }

        public async Task<AuthResponseDto?> RegisterAsync(UserRegistrationDto registrationDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(registrationDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(ServiceConstants.ApiEndpoints.Register, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<AuthResponseDto>(responseJson, _jsonOptions);
                }
                else
                {
                    _logger.LogWarning("Registration failed. Status: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
            }
            return null;
        }

        public async Task<AuthResponseDto?> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(refreshTokenDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(ServiceConstants.ApiEndpoints.RefreshToken, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<AuthResponseDto>(responseJson, _jsonOptions);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
            }
            return null;
        }

        public async Task<bool> LogoutAsync()
        {
            try
            {
                var response = await _httpClient.PostAsync(ServiceConstants.ApiEndpoints.Logout, null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return false;
            }
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(forgotPasswordDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(ServiceConstants.ApiEndpoints.ForgotPassword, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending forgot password request");
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(resetPasswordDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(ServiceConstants.ApiEndpoints.ResetPassword, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password");
                return false;
            }
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(changePasswordDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(ServiceConstants.ApiEndpoints.ChangePassword, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                return false;
            }
        }

        public async Task<UserDto?> GetCurrentUserAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(ServiceConstants.ApiEndpoints.UserProfile);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<UserDto>(json, _jsonOptions);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user");
            }
            return null;
        }

        public async Task<UserDto?> GetUserAsync(string userId)
        {
            try
            {
                var endpoint = string.Format(ServiceConstants.ApiEndpoints.GetUser, userId);
                var response = await _httpClient.GetAsync(endpoint);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<UserDto>(json, _jsonOptions);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user {UserId}", userId);
            }
            return null;
        }

        public async Task<bool> UpdateProfileAsync(UserDto userDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(userDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(ServiceConstants.ApiEndpoints.UserProfile, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile");
                return false;
            }
        }

        // For testing purposes - in real implementation, check JWT token validity
        public async Task<bool> IsAuthenticatedAsync()
        {
            await Task.CompletedTask;
            return true; // Stubbed for testing
        }

        public async Task<string?> GetCurrentUserIdAsync()
        {
            await Task.CompletedTask;
            return "test-user-id"; // Stubbed for testing
        }

        public async Task<string?> GetTokenAsync()
        {
            await Task.CompletedTask;
            return "test-token"; // Stubbed for testing
        }

        public void SetToken(string token)
        {
            // In real implementation, store in secure storage
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(ServiceConstants.Auth.BearerScheme, token);
        }

        public void ClearToken()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}

