using StockTradePro.BlazorUI.Models.Common;
using StockTradePro.BlazorUI.Models.Portfolio;

namespace StockTradePro.BlazorUI.Services
{
    public interface IPortfolioService
    {
        Task<PaginatedResult<PortfolioSummaryDto>> GetPortfoliosAsync(int page = 1, int pageSize = 10);
        Task<PortfolioDto?> GetPortfolioAsync(int id);
        Task<PortfolioDto?> CreatePortfolioAsync(CreatePortfolioDto createDto);
        Task<PortfolioDto?> UpdatePortfolioAsync(int id, UpdatePortfolioDto updateDto);
        Task<bool> DeletePortfolioAsync(int id);
        Task<bool> RecalculatePortfolioAsync(int id);
        Task<List<PortfolioSummaryDto>> GetPublicPortfoliosAsync(int page = 1, int pageSize = 20);
        Task<PaginatedResult<TransactionDto>> GetPortfolioTransactionsAsync(int portfolioId, int page = 1, int pageSize = 20);
        Task<TransactionDto?> GetTransactionAsync(int id);
        Task<TransactionDto?> CreateTransactionAsync(CreateTransactionDto createDto);
        Task<bool> DeleteTransactionAsync(int id);
        Task<List<TransactionDto>> GetSymbolTransactionsAsync(int portfolioId, string symbol);
        Task<List<TransactionDto>> GetRecentTransactionsAsync(int count = 10);
    }
}
