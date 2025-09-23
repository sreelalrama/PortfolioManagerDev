using StockTradePro.Portfolio.API.Models.DTOs;

namespace StockTradePro.Portfolio.API.Services
{
    public interface IHoldingService
    {
        Task<List<HoldingDto>> GetPortfolioHoldingsAsync(int portfolioId, string userId);
        Task<HoldingDto?> GetHoldingBySymbolAsync(int portfolioId, string symbol, string userId);
        Task UpdateHoldingPricesAsync(int portfolioId);
        Task UpdateAllHoldingPricesAsync();
        Task RecalculateHoldingsAsync(int portfolioId);
    }
}