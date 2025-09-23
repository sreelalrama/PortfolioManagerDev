
using System.Text.Json;

namespace StockTradePro.NotificationService.API.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserService> _logger;

        public UserService(HttpClient httpClient, IConfiguration configuration, ILogger<UserService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string?> GetUserEmailAsync(string userId)
        {
            var user = await GetUserAsync(userId);
            return user?.Email;
        }

        public async Task<UserDto?> GetUserAsync(string userId)
        {
            try
            {
                var userServiceUrl = _configuration["Services:UserManagementService"];
                var response = await _httpClient.GetAsync($"{userServiceUrl}/api/users/{userId}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<UserDto>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user data for user {UserId}", userId);
            }

            return null;
        }

        public async Task<IEnumerable<string>> GetAllUserIdsAsync()
        {
            try
            {
                var userServiceUrl = _configuration["Services:UserManagementService"];
                var response = await _httpClient.GetAsync($"{userServiceUrl}/api/users");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var users = JsonSerializer.Deserialize<IEnumerable<UserDto>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return users?.Select(u => u.Id) ?? Enumerable.Empty<string>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all user IDs");
            }

            return Enumerable.Empty<string>();
        }
    }
}