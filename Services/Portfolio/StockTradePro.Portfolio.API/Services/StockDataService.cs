using StockTradePro.Portfolio.API.Models.DTOs;
using System.Text.Json;

namespace StockTradePro.Portfolio.API.Services
{
    public class StockDataService : IStockDataService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<StockDataService> _logger;

        public StockDataService(IHttpClientFactory httpClientFactory, ILogger<StockDataService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("ApiGateway");
            _logger = logger;
        }

        public async Task<decimal?> GetCurrentStockPriceAsync(string symbol)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/prices/{symbol}/current");
                if (!response.IsSuccessStatusCode) return null;

                var content = await response.Content.ReadAsStringAsync();
                var priceData = JsonSerializer.Deserialize<JsonElement>(content);

                if (priceData.TryGetProperty("currentPrice", out var priceElement))
                {
                    return priceElement.GetDecimal();
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current stock price for symbol: {Symbol}", symbol);
                return null;
            }
        }

        public async Task<Dictionary<string, decimal>> GetMultipleStockPricesAsync(List<string> symbols)
        {
            var prices = new Dictionary<string, decimal>();

            // For now, make individual requests. Could be optimized with bulk endpoint
            var tasks = symbols.Select(async symbol =>
            {
                var price = await GetCurrentStockPriceAsync(symbol);
                if (price.HasValue)
                {
                    lock (prices)
                    {
                        prices[symbol] = price.Value;
                    }
                }
            });

            await Task.WhenAll(tasks);
            return prices;
        }

        public async Task<StockInfoDto?> GetStockInfoAsync(string symbol)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/stocks/{symbol}");
                if (!response.IsSuccessStatusCode) return null;

                var content = await response.Content.ReadAsStringAsync();
                var stockData = JsonSerializer.Deserialize<JsonElement>(content);

                return new StockInfoDto
                {
                    Symbol = stockData.GetProperty("symbol").GetString() ?? symbol,
                    CompanyName = stockData.GetProperty("companyName").GetString() ?? symbol,
                    CurrentPrice = stockData.GetProperty("currentPrice").GetDecimal(),
                    PriceChange = stockData.GetProperty("priceChange").GetDecimal(),
                    PriceChangePercent = stockData.GetProperty("priceChangePercent").GetDecimal(),
                    Volume = stockData.GetProperty("volume").GetInt64(),
                    LastUpdated = stockData.GetProperty("lastUpdated").GetDateTime()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock info for symbol: {Symbol}", symbol);
                return null;
            }
        }
    }
}