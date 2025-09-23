using StockTradePro.BlazorUI.Constants;
using StockTradePro.BlazorUI.Models.Common;
using StockTradePro.BlazorUI.Models.Portfolio;
using System.Text;

namespace StockTradePro.BlazorUI.Services
{
   public class PortfolioService : IPortfolioService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ILogger<PortfolioService> _logger;

        public PortfolioService(HttpClient httpClient, ILogger<PortfolioService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<PaginatedResult<PortfolioSummaryDto>> GetPortfoliosAsync(int page = 1, int pageSize = 10)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ServiceConstants.ApiEndpoints.GetPortfolios}?page={page}&pageSize={pageSize}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<PaginatedResult<PortfolioSummaryDto>>(json, _jsonOptions) ?? new PaginatedResult<PortfolioSummaryDto>();
                }
                else
                {
                    _logger.LogWarning("Failed to get portfolios. Status: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting portfolios");
            }
            return new PaginatedResult<PortfolioSummaryDto>();
        }

        public async Task<PortfolioDto?> GetPortfolioAsync(int id)
        {
            try
            {
                var endpoint = string.Format(ServiceConstants.ApiEndpoints.GetPortfolio, id);
                var response = await _httpClient.GetAsync(endpoint);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<PortfolioDto>(json, _jsonOptions);
                }
                else
                {
                    _logger.LogWarning("Failed to get portfolio {Id}. Status: {StatusCode}", id, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting portfolio {Id}", id);
            }
            return null;
        }

        public async Task<PortfolioDto?> CreatePortfolioAsync(CreatePortfolioDto createDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(createDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(ServiceConstants.ApiEndpoints.CreatePortfolio, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<PortfolioDto>(responseJson, _jsonOptions);
                }
                else
                {
                    _logger.LogWarning("Failed to create portfolio. Status: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating portfolio");
            }
            return null;
        }

        public async Task<PortfolioDto?> UpdatePortfolioAsync(int id, UpdatePortfolioDto updateDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(updateDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var endpoint = string.Format(ServiceConstants.ApiEndpoints.UpdatePortfolio, id);

                var response = await _httpClient.PutAsync(endpoint, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<PortfolioDto>(responseJson, _jsonOptions);
                }
                else
                {
                    _logger.LogWarning("Failed to update portfolio {Id}. Status: {StatusCode}", id, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating portfolio {Id}", id);
            }
            return null;
        }

        public async Task<bool> DeletePortfolioAsync(int id)
        {
            try
            {
                var endpoint = string.Format(ServiceConstants.ApiEndpoints.DeletePortfolio, id);
                var response = await _httpClient.DeleteAsync(endpoint);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting portfolio {Id}", id);
                return false;
            }
        }

        public async Task<bool> RecalculatePortfolioAsync(int id)
        {
            try
            {
                var endpoint = string.Format(ServiceConstants.ApiEndpoints.RecalculatePortfolio, id);
                var response = await _httpClient.PostAsync(endpoint, null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recalculating portfolio {Id}", id);
                return false;
            }
        }

        public async Task<List<PortfolioSummaryDto>> GetPublicPortfoliosAsync(int page = 1, int pageSize = 20)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ServiceConstants.ApiEndpoints.GetPublicPortfolios}?page={page}&pageSize={pageSize}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<PortfolioSummaryDto>>(json, _jsonOptions) ?? new List<PortfolioSummaryDto>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting public portfolios");
            }
            return new List<PortfolioSummaryDto>();
        }

        public async Task<PaginatedResult<TransactionDto>> GetPortfolioTransactionsAsync(int portfolioId, int page = 1, int pageSize = 20)
        {
            try
            {
                var endpoint = string.Format(ServiceConstants.ApiEndpoints.GetPortfolioTransactions, portfolioId);
                var response = await _httpClient.GetAsync($"{endpoint}?page={page}&pageSize={pageSize}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<PaginatedResult<TransactionDto>>(json, _jsonOptions) ?? new PaginatedResult<TransactionDto>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transactions for portfolio {PortfolioId}", portfolioId);
            }
            return new PaginatedResult<TransactionDto>();
        }

        public async Task<TransactionDto?> GetTransactionAsync(int id)
        {
            try
            {
                var endpoint = string.Format(ServiceConstants.ApiEndpoints.GetTransaction, id);
                var response = await _httpClient.GetAsync(endpoint);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<TransactionDto>(json, _jsonOptions);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transaction {Id}", id);
            }
            return null;
        }

        public async Task<TransactionDto?> CreateTransactionAsync(CreateTransactionDto createDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(createDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(ServiceConstants.ApiEndpoints.CreateTransaction, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<TransactionDto>(responseJson, _jsonOptions);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating transaction");
            }
            return null;
        }

        public async Task<bool> DeleteTransactionAsync(int id)
        {
            try
            {
                var endpoint = string.Format(ServiceConstants.ApiEndpoints.DeleteTransaction, id);
                var response = await _httpClient.DeleteAsync(endpoint);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting transaction {Id}", id);
                return false;
            }
        }

        public async Task<List<TransactionDto>> GetSymbolTransactionsAsync(int portfolioId, string symbol)
        {
            try
            {
                var endpoint = string.Format(ServiceConstants.ApiEndpoints.GetSymbolTransactions, portfolioId, symbol);
                var response = await _httpClient.GetAsync(endpoint);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<TransactionDto>>(json, _jsonOptions) ?? new List<TransactionDto>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transactions for portfolio {PortfolioId} symbol {Symbol}", portfolioId, symbol);
            }
            return new List<TransactionDto>();
        }

        public async Task<List<TransactionDto>> GetRecentTransactionsAsync(int count = 10)
        {
            // For now, return empty list - we'll implement this when we have portfolio details
            await Task.CompletedTask;
            return new List<TransactionDto>();
        }
    }
}
