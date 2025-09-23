using StockTradePro.Portfolio.API.Models.DTOs;

namespace StockTradePro.Portfolio.API.Services
{
    public interface IPerformanceService
    {
        Task<List<PortfolioPerformanceDto>> GetPortfolioPerformanceAsync(int portfolioId, string userId, int days = 30);
        Task CalculateAndSavePerformanceAsync(int portfolioId);
        Task CalculateAllPortfolioPerformancesAsync();
        Task<Dictionary<string, decimal>> GetPortfolioMetricsAsync(int portfolioId, string userId);
    }
}