using StockTradePro.BlazorUI.Constants;
using StockTradePro.BlazorUI.Models.Watchlist;
using System.Text;

namespace StockTradePro.BlazorUI.Services
{
    public class WatchlistService : IWatchlistService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ILogger<WatchlistService> _logger;
        private readonly IAuthService _authService;

        public WatchlistService(HttpClient httpClient, ILogger<WatchlistService> logger, IAuthService authService)
        {
            _httpClient = httpClient;
            _logger = logger;
            _authService = authService;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        private async Task AddAuthHeader()
        {
            var token = await _authService.GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue(ServiceConstants.Auth.BearerScheme, token);
            }
        }

        public async Task<List<WatchlistDto>> GetWatchlistsAsync()
        {
            try
            {
                await AddAuthHeader();
                var response = await _httpClient.GetAsync(ServiceConstants.ApiEndpoints.GetWatchlists);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<WatchlistDto>>(json, _jsonOptions) ?? new List<WatchlistDto>();
                }
                else
                {
                    _logger.LogWarning("Failed to get watchlists. Status: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting watchlists");
            }
            return new List<WatchlistDto>();
        }

        public async Task<WatchlistDto?> GetWatchlistAsync(int id)
        {
            try
            {
                await AddAuthHeader();
                var endpoint = string.Format(ServiceConstants.ApiEndpoints.GetWatchlist, id);
                var response = await _httpClient.GetAsync(endpoint);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<WatchlistDto>(json, _jsonOptions);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting watchlist {Id}", id);
            }
            return null;
        }

        public async Task<WatchlistDto?> CreateWatchlistAsync(CreateWatchlistDto createDto)
        {
            try
            {
                await AddAuthHeader();
                var json = JsonSerializer.Serialize(createDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(ServiceConstants.ApiEndpoints.CreateWatchlist, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<WatchlistDto>(responseJson, _jsonOptions);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating watchlist");
            }
            return null;
        }

        public async Task<WatchlistDto?> UpdateWatchlistAsync(int id, UpdateWatchlistDto updateDto)
        {
            try
            {
                await AddAuthHeader();
                var json = JsonSerializer.Serialize(updateDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var endpoint = string.Format(ServiceConstants.ApiEndpoints.UpdateWatchlist, id);

                var response = await _httpClient.PutAsync(endpoint, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<WatchlistDto>(responseJson, _jsonOptions);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating watchlist {Id}", id);
            }
            return null;
        }

        public async Task<bool> DeleteWatchlistAsync(int id)
        {
            try
            {
                await AddAuthHeader();
                var endpoint = string.Format(ServiceConstants.ApiEndpoints.DeleteWatchlist, id);
                var response = await _httpClient.DeleteAsync(endpoint);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting watchlist {Id}", id);
                return false;
            }
        }

        public async Task<List<WatchlistItemDto>> GetWatchlistItemsAsync(int watchlistId)
        {
            try
            {
                await AddAuthHeader();
                var endpoint = string.Format(ServiceConstants.ApiEndpoints.GetWatchlistItems, watchlistId);
                var response = await _httpClient.GetAsync(endpoint);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<WatchlistItemDto>>(json, _jsonOptions) ?? new List<WatchlistItemDto>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting watchlist items for {WatchlistId}", watchlistId);
            }
            return new List<WatchlistItemDto>();
        }

        public async Task<WatchlistItemDto?> AddWatchlistItemAsync(int watchlistId, AddWatchlistItemDto addDto)
        {
            try
            {
                await AddAuthHeader();
                var json = JsonSerializer.Serialize(addDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var endpoint = string.Format(ServiceConstants.ApiEndpoints.AddWatchlistItem, watchlistId);

                var response = await _httpClient.PostAsync(endpoint, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<WatchlistItemDto>(responseJson, _jsonOptions);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to watchlist {WatchlistId}", watchlistId);
            }
            return null;
        }

        public async Task<bool> DeleteWatchlistItemAsync(int watchlistId, int itemId)
        {
            try
            {
                await AddAuthHeader();
                var endpoint = string.Format(ServiceConstants.ApiEndpoints.DeleteWatchlistItem, watchlistId, itemId);
                var response = await _httpClient.DeleteAsync(endpoint);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting watchlist item {ItemId} from watchlist {WatchlistId}", itemId, watchlistId);
                return false;
            }
        }

        public async Task<bool> UpdateItemSortOrderAsync(int watchlistId, int itemId, int sortOrder)
        {
            try
            {
                await AddAuthHeader();
                var json = JsonSerializer.Serialize(sortOrder, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var endpoint = string.Format(ServiceConstants.ApiEndpoints.UpdateItemSortOrder, watchlistId, itemId);

                var response = await _httpClient.PatchAsync(endpoint, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating sort order for item {ItemId} in watchlist {WatchlistId}", itemId, watchlistId);
                return false;
            }
        }

        public async Task<List<PriceAlertDto>> GetPriceAlertsAsync(int watchlistId)
        {
            try
            {
                await AddAuthHeader();
                var endpoint = string.Format(ServiceConstants.ApiEndpoints.GetPriceAlerts, watchlistId);
                var response = await _httpClient.GetAsync(endpoint);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<PriceAlertDto>>(json, _jsonOptions) ?? new List<PriceAlertDto>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting price alerts for watchlist {WatchlistId}", watchlistId);
            }
            return new List<PriceAlertDto>();
        }

        public async Task<PriceAlertDto?> GetPriceAlertAsync(int watchlistId, int alertId)
        {
            try
            {
                await AddAuthHeader();
                var endpoint = string.Format(ServiceConstants.ApiEndpoints.GetPriceAlert, watchlistId, alertId);
                var response = await _httpClient.GetAsync(endpoint);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<PriceAlertDto>(json, _jsonOptions);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting price alert {AlertId} for watchlist {WatchlistId}", alertId, watchlistId);
            }
            return null;
        }

        public async Task<PriceAlertDto?> CreatePriceAlertAsync(int watchlistId, CreatePriceAlertDto createDto)
        {
            try
            {
                await AddAuthHeader();
                var json = JsonSerializer.Serialize(createDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var endpoint = string.Format(ServiceConstants.ApiEndpoints.CreatePriceAlert, watchlistId);

                var response = await _httpClient.PostAsync(endpoint, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<PriceAlertDto>(responseJson, _jsonOptions);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating price alert for watchlist {WatchlistId}", watchlistId);
            }
            return null;
        }

        public async Task<bool> DeletePriceAlertAsync(int watchlistId, int alertId)
        {
            try
            {
                await AddAuthHeader();
                var endpoint = string.Format(ServiceConstants.ApiEndpoints.DeletePriceAlert, watchlistId, alertId);
                var response = await _httpClient.DeleteAsync(endpoint);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting price alert {AlertId} from watchlist {WatchlistId}", alertId, watchlistId);
                return false;
            }
        }

        public async Task<bool> DisablePriceAlertAsync(int watchlistId, int alertId)
        {
            try
            {
                await AddAuthHeader();
                var endpoint = string.Format(ServiceConstants.ApiEndpoints.DisablePriceAlert, watchlistId, alertId);
                var response = await _httpClient.PatchAsync(endpoint, null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disabling price alert {AlertId} for watchlist {WatchlistId}", alertId, watchlistId);
                return false;
            }
        }

        public async Task<List<PriceAlertDto>> GetActiveAlertsAsync()
        {
            await AddAuthHeader();
            // For now, return empty list - we'll implement this when we have active alerts logic
            await Task.CompletedTask;
            return new List<PriceAlertDto>();
        }
    }
}
