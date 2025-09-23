
using System.Text.Json;
using StockTradePro.WatchlistService.API.DTOs;

namespace StockTradePro.WatchlistService.API.Services
{
    public class StockDataService : IStockDataService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<StockDataService> _logger;

        public StockDataService(HttpClient httpClient, IConfiguration configuration, ILogger<StockDataService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<StockDto?> GetStockAsync(string symbol)
        {
            try
            {
                var stockDataServiceUrl = _configuration["Services:StockDataService"];
                var response = await _httpClient.GetAsync($"{stockDataServiceUrl}/api/stocks/{symbol}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<StockDto>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get stock data for symbol {Symbol}", symbol);
            }

            return null;
        }

        public async Task<StockPriceDto?> GetCurrentPriceAsync(string symbol)
        {
            try
            {
                var stockDataServiceUrl = _configuration["Services:StockDataService"];
                var response = await _httpClient.GetAsync($"{stockDataServiceUrl}/api/stocks/{symbol}/price");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<StockPriceDto>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get current price for symbol {Symbol}", symbol);
            }

            return null;
        }
    }
}