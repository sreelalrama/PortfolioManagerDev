using StockTradePro.Portfolio.API.Models.DTOs;

public interface IPortfolioService
{
    Task<PaginatedResult<PortfolioSummaryDto>> GetUserPortfoliosAsync(string userId, int page = 1, int pageSize = 10);
    Task<PortfolioDto?> GetPortfolioByIdAsync(int portfolioId, string userId);
    Task<PortfolioDto> CreatePortfolioAsync(string userId, CreatePortfolioDto createPortfolioDto);
    Task<PortfolioDto?> UpdatePortfolioAsync(int portfolioId, string userId, UpdatePortfolioDto updatePortfolioDto);
    Task<bool> DeletePortfolioAsync(int portfolioId, string userId);
    Task<bool> PortfolioExistsAsync(int portfolioId, string userId);
    Task<List<PortfolioSummaryDto>> GetPublicPortfoliosAsync(int page = 1, int pageSize = 20);
    Task RecalculatePortfolioValueAsync(int portfolioId);
}