using StockTradePro.Portfolio.API.Models.DTOs;

namespace StockTradePro.Portfolio.API.Services
{
    public interface ITransactionService
    {
        Task<PaginatedResult<TransactionDto>> GetPortfolioTransactionsAsync(int portfolioId, string userId, int page = 1, int pageSize = 20);
        Task<TransactionDto?> GetTransactionByIdAsync(int transactionId, string userId);
        Task<TransactionDto> CreateTransactionAsync(string userId, CreateTransactionDto createTransactionDto);
        Task<bool> DeleteTransactionAsync(int transactionId, string userId);
        Task<List<TransactionDto>> GetTransactionsBySymbolAsync(int portfolioId, string symbol, string userId);
    }
}
