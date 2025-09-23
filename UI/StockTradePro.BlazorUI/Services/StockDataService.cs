using StockTradePro.BlazorUI.Constants;
using StockTradePro.BlazorUI.Models.Common;
using StockTradePro.BlazorUI.Models.Stocks;

namespace StockTradePro.BlazorUI.Services
{
    public class StockDataService : IStockDataService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ILogger<StockDataService> _logger;

        public StockDataService(HttpClient httpClient, ILogger<StockDataService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<MarketOverviewDto?> GetMarketOverviewAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(ServiceConstants.ApiEndpoints.MarketOverview);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<MarketOverviewDto>(json, _jsonOptions);
                }
                else
                {
                    _logger.LogWarning("Failed to get market overview. Status: {StatusCode}", response.StatusCode);
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error getting market overview");
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout getting market overview");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting market overview");
            }
            return null;
        }

        public async Task<List<StockDto>> GetTrendingStocksAsync(int count = ServiceConstants.Defaults.TopStocksCount)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ServiceConstants.ApiEndpoints.TrendingStocks}?count={count}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<StockDto>>(json, _jsonOptions) ?? new List<StockDto>();
                }
                else
                {
                    _logger.LogWarning("Failed to get trending stocks. Status: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting trending stocks");
            }
            return new List<StockDto>();
        }

        public async Task<List<StockDto>> GetTopGainersAsync(int count = ServiceConstants.Defaults.TopStocksCount)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ServiceConstants.ApiEndpoints.TopGainers}?count={count}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<StockDto>>(json, _jsonOptions) ?? new List<StockDto>();
                }
                else
                {
                    _logger.LogWarning("Failed to get top gainers. Status: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top gainers");
            }
            return new List<StockDto>();
        }

        public async Task<List<StockDto>> GetTopLosersAsync(int count = ServiceConstants.Defaults.TopStocksCount)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ServiceConstants.ApiEndpoints.TopLosers}?count={count}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<StockDto>>(json, _jsonOptions) ?? new List<StockDto>();
                }
                else
                {
                    _logger.LogWarning("Failed to get top losers. Status: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top losers");
            }
            return new List<StockDto>();
        }

        public async Task<List<StockDto>> GetMostActiveAsync(int count = ServiceConstants.Defaults.TopStocksCount)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ServiceConstants.ApiEndpoints.MostActive}?count={count}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<StockDto>>(json, _jsonOptions) ?? new List<StockDto>();
                }
                else
                {
                    _logger.LogWarning("Failed to get most active stocks. Status: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting most active stocks");
            }
            return new List<StockDto>();
        }

        public async Task<StockDto?> GetStockAsync(string symbol)
        {
            try
            {
                var endpoint = string.Format(ServiceConstants.ApiEndpoints.GetStock, symbol);
                var response = await _httpClient.GetAsync(endpoint);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<StockDto>(json, _jsonOptions);
                }
                else
                {
                    _logger.LogWarning("Failed to get stock {Symbol}. Status: {StatusCode}", symbol, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock {Symbol}", symbol);
            }
            return null;
        }

        public async Task<PaginatedResult<StockDto>> GetStocksAsync(StockSearchDto searchDto)
        {
            try
            {
                var queryParams = new List<string>();

                if (!string.IsNullOrEmpty(searchDto.Query))
                    queryParams.Add($"Query={Uri.EscapeDataString(searchDto.Query)}");
                if (!string.IsNullOrEmpty(searchDto.Sector))
                    queryParams.Add($"Sector={Uri.EscapeDataString(searchDto.Sector)}");
                if (!string.IsNullOrEmpty(searchDto.Exchange))
                    queryParams.Add($"Exchange={Uri.EscapeDataString(searchDto.Exchange)}");
                if (searchDto.MinPrice.HasValue)
                    queryParams.Add($"MinPrice={searchDto.MinPrice}");
                if (searchDto.MaxPrice.HasValue)
                    queryParams.Add($"MaxPrice={searchDto.MaxPrice}");
                if (!string.IsNullOrEmpty(searchDto.SortBy))
                    queryParams.Add($"SortBy={searchDto.SortBy}");

                queryParams.Add($"SortOrder={searchDto.SortOrder}");
                queryParams.Add($"Page={searchDto.Page}");
                queryParams.Add($"PageSize={searchDto.PageSize}");

                var queryString = string.Join("&", queryParams);
                var endpoint = $"{ServiceConstants.ApiEndpoints.GetStocks}?{queryString}";

                var response = await _httpClient.GetAsync(endpoint);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<PaginatedResult<StockDto>>(json, _jsonOptions) ?? new PaginatedResult<StockDto>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stocks");
            }
            return new PaginatedResult<StockDto>();
        }

        public async Task<PaginatedResult<StockDto>> SearchStocksAsync(string query, int page = 1, int pageSize = 20)
        {
            try
            {
                var endpoint = $"{ServiceConstants.ApiEndpoints.SearchStocks}?query={Uri.EscapeDataString(query)}&page={page}&pageSize={pageSize}";
                var response = await _httpClient.GetAsync(endpoint);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<PaginatedResult<StockDto>>(json, _jsonOptions) ?? new PaginatedResult<StockDto>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching stocks");
            }
            return new PaginatedResult<StockDto>();
        }

        public async Task<List<string>> GetSectorsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(ServiceConstants.ApiEndpoints.GetSectors);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<string>>(json, _jsonOptions) ?? new List<string>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sectors");
            }
            return new List<string>();
        }

        public async Task<List<string>> GetExchangesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(ServiceConstants.ApiEndpoints.GetExchanges);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<string>>(json, _jsonOptions) ?? new List<string>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting exchanges");
            }
            return new List<string>();
        }

        public async Task<List<StockPriceDto>> GetPricesAsync(string symbol, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var endpoint = string.Format(ServiceConstants.ApiEndpoints.GetPrices, symbol);
                var queryParams = new List<string>();

                if (fromDate.HasValue)
                    queryParams.Add($"fromDate={fromDate.Value:yyyy-MM-ddTHH:mm:ss}");
                if (toDate.HasValue)
                    queryParams.Add($"toDate={toDate.Value:yyyy-MM-ddTHH:mm:ss}");

                if (queryParams.Any())
                    endpoint += "?" + string.Join("&", queryParams);

                var response = await _httpClient.GetAsync(endpoint);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<StockPriceDto>>(json, _jsonOptions) ?? new List<StockPriceDto>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting prices for {Symbol}", symbol);
            }
            return new List<StockPriceDto>();
        }

        public async Task<StockPriceDto?> GetCurrentPriceAsync(string symbol)
        {
            try
            {
                var endpoint = string.Format(ServiceConstants.ApiEndpoints.GetCurrentPrice, symbol);
                var response = await _httpClient.GetAsync(endpoint);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<StockPriceDto>(json, _jsonOptions);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current price for {Symbol}", symbol);
            }
            return null;
        }
    }
}
