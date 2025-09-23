using StockTradePro.BlazorUI.Constants;
using StockTradePro.BlazorUI.Models.Notifications;
using System.Text;

namespace StockTradePro.BlazorUI.Services
{
    public class NotificationService : INotificationService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(HttpClient httpClient, ILogger<NotificationService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<NotificationDto>> GetNotificationsAsync(int page = 1, int pageSize = 20)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ServiceConstants.ApiEndpoints.GetNotifications}?page={page}&pageSize={pageSize}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<NotificationDto>>(json, _jsonOptions) ?? new List<NotificationDto>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notifications");
            }
            return new List<NotificationDto>();
        }

        public async Task<NotificationDto?> GetNotificationAsync(int id)
        {
            try
            {
                var endpoint = string.Format(ServiceConstants.ApiEndpoints.GetNotification, id);
                var response = await _httpClient.GetAsync(endpoint);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<NotificationDto>(json, _jsonOptions);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notification {Id}", id);
            }
            return null;
        }

        public async Task<NotificationDto?> CreateNotificationAsync(CreateNotificationDto createDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(createDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(ServiceConstants.ApiEndpoints.CreateNotification, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<NotificationDto>(responseJson, _jsonOptions);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notification");
            }
            return null;
        }

        public async Task<bool> DeleteNotificationAsync(int id)
        {
            try
            {
                var endpoint = string.Format(ServiceConstants.ApiEndpoints.DeleteNotification, id);
                var response = await _httpClient.DeleteAsync(endpoint);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification {Id}", id);
                return false;
            }
        }

        public async Task<bool> MarkNotificationReadAsync(int id)
        {
            try
            {
                var endpoint = string.Format(ServiceConstants.ApiEndpoints.MarkNotificationRead, id);
                var response = await _httpClient.PatchAsync(endpoint, null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification {Id} as read", id);
                return false;
            }
        }

        public async Task<bool> MarkAllNotificationsReadAsync()
        {
            try
            {
                var response = await _httpClient.PatchAsync(ServiceConstants.ApiEndpoints.MarkAllNotificationsRead, null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking all notifications as read");
                return false;
            }
        }

        public async Task<int> GetUnreadCountAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(ServiceConstants.ApiEndpoints.GetUnreadCount);
                if (response.IsSuccessStatusCode)
                {
                    var countStr = await response.Content.ReadAsStringAsync();
                    return int.TryParse(countStr, out var count) ? count : 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread count");
            }
            return 0;
        }

        public async Task<List<NotificationPreferenceDto>> GetNotificationPreferencesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(ServiceConstants.ApiEndpoints.GetNotificationPreferences);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<NotificationPreferenceDto>>(json, _jsonOptions) ?? new List<NotificationPreferenceDto>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notification preferences");
            }
            return new List<NotificationPreferenceDto>();
        }

        public async Task<NotificationPreferenceDto?> GetNotificationPreferenceAsync(NotificationType type)
        {
            try
            {
                var endpoint = string.Format(ServiceConstants.ApiEndpoints.GetNotificationPreference, (int)type);
                var response = await _httpClient.GetAsync(endpoint);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<NotificationPreferenceDto>(json, _jsonOptions);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notification preference for type {Type}", type);
            }
            return null;
        }

        public async Task<NotificationPreferenceDto?> UpdateNotificationPreferenceAsync(NotificationType type, UpdateNotificationPreferenceDto updateDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(updateDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var endpoint = string.Format(ServiceConstants.ApiEndpoints.UpdateNotificationPreference, (int)type);

                var response = await _httpClient.PutAsync(endpoint, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<NotificationPreferenceDto>(responseJson, _jsonOptions);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating notification preference for type {Type}", type);
            }
            return null;
        }

        public async Task<bool> InitializePreferencesAsync()
        {
            try
            {
                var response = await _httpClient.PostAsync(ServiceConstants.ApiEndpoints.InitializePreferences, null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing notification preferences");
                return false;
            }
        }
    }
}
