using Microsoft.EntityFrameworkCore;
using StockTradePro.Portfolio.API.Data;
using StockTradePro.Portfolio.API.Models.DTOs;
using StockTradePro.Portfolio.API.Models.Entities;

namespace StockTradePro.Portfolio.API.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly PortfolioDbContext _context;
        private readonly IHoldingService _holdingService;
        private readonly IStockDataService _stockDataService;
        private readonly ILogger<TransactionService> _logger;

        public TransactionService(
            PortfolioDbContext context,
            IHoldingService holdingService,
            IStockDataService stockDataService,
            ILogger<TransactionService> logger)
        {
            _context = context;
            _holdingService = holdingService;
            _stockDataService = stockDataService;
            _logger = logger;
        }

        public async Task<PaginatedResult<TransactionDto>> GetPortfolioTransactionsAsync(
            int portfolioId, string userId, int page = 1, int pageSize = 20)
        {
            try
            {
                // First verify user owns the portfolio
                var portfolioExists = await _context.Portfolios
                    .AnyAsync(p => p.Id == portfolioId && p.UserId == userId && p.IsActive);

                if (!portfolioExists)
                    return new PaginatedResult<TransactionDto>();

                var query = _context.Transactions
                    .Where(t => t.PortfolioId == portfolioId)
                    .OrderByDescending(t => t.TransactionDate);

                var totalRecords = await query.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                var transactions = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var transactionDtos = await MapToTransactionDtos(transactions);

                return new PaginatedResult<TransactionDto>
                {
                    Data = transactionDtos,
                    TotalRecords = totalRecords,
                    PageNumber = page,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    HasNextPage = page < totalPages,
                    HasPreviousPage = page > 1
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting portfolio transactions");
                throw;
            }
        }

        public async Task<TransactionDto?> GetTransactionByIdAsync(int transactionId, string userId)
        {
            try
            {
                var transaction = await _context.Transactions
                    .Include(t => t.Portfolio)
                    .FirstOrDefaultAsync(t => t.Id == transactionId && t.Portfolio.UserId == userId);

                if (transaction == null) return null;

                var transactionDtos = await MapToTransactionDtos(new[] { transaction });
                return transactionDtos.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transaction by ID: {TransactionId}", transactionId);
                throw;
            }
        }

        public async Task<TransactionDto> CreateTransactionAsync(string userId, CreateTransactionDto createTransactionDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Verify user owns the portfolio
                var portfolio = await _context.Portfolios
                    .FirstOrDefaultAsync(p => p.Id == createTransactionDto.PortfolioId && p.UserId == userId && p.IsActive);

                if (portfolio == null)
                    throw new InvalidOperationException("Portfolio not found or access denied");

                // Get stock info to validate symbol
                var stockInfo = await _stockDataService.GetStockInfoAsync(createTransactionDto.Symbol);
                if (stockInfo == null)
                    throw new InvalidOperationException($"Invalid stock symbol: {createTransactionDto.Symbol}");

                // For SELL transactions, check if user has enough shares
                if (createTransactionDto.Type == "SELL")
                {
                    var currentHolding = await _context.Holdings
                        .FirstOrDefaultAsync(h => h.PortfolioId == createTransactionDto.PortfolioId
                                                 && h.Symbol == createTransactionDto.Symbol.ToUpper());

                    if (currentHolding == null || currentHolding.Quantity < createTransactionDto.Quantity)
                    {
                        throw new InvalidOperationException("Insufficient shares for sell transaction");
                    }
                }

                // Calculate transaction amounts
                var totalAmount = createTransactionDto.Quantity * createTransactionDto.Price;
                var netAmount = createTransactionDto.Type == "BUY"
                    ? totalAmount + createTransactionDto.Fees
                    : totalAmount - createTransactionDto.Fees;

                var newTransaction = new Transaction
                {
                    PortfolioId = createTransactionDto.PortfolioId,
                    Symbol = createTransactionDto.Symbol.ToUpper(),
                    Type = createTransactionDto.Type,
                    Quantity = createTransactionDto.Quantity,
                    Price = createTransactionDto.Price,
                    TotalAmount = totalAmount,
                    Fees = createTransactionDto.Fees,
                    NetAmount = netAmount,
                    Notes = createTransactionDto.Notes,
                    TransactionDate = createTransactionDto.TransactionDate ?? DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Transactions.Add(newTransaction);
                await _context.SaveChangesAsync();

                // Update holdings
                await _holdingService.RecalculateHoldingsAsync(createTransactionDto.PortfolioId);

                await transaction.CommitAsync();

                var transactionDtos = await MapToTransactionDtos(new[] { newTransaction });
                return transactionDtos.First();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteTransactionAsync(int transactionId, string userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var transactionEntity = await _context.Transactions
                    .Include(t => t.Portfolio)
                    .FirstOrDefaultAsync(t => t.Id == transactionId && t.Portfolio.UserId == userId);

                if (transactionEntity == null) return false;

                _context.Transactions.Remove(transactionEntity);
                await _context.SaveChangesAsync();

                // Recalculate holdings after transaction removal
                await _holdingService.RecalculateHoldingsAsync(transactionEntity.PortfolioId);

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<TransactionDto>> GetTransactionsBySymbolAsync(int portfolioId, string symbol, string userId)
        {
            try
            {
                // Verify user owns the portfolio
                var portfolioExists = await _context.Portfolios
                    .AnyAsync(p => p.Id == portfolioId && p.UserId == userId && p.IsActive);

                if (!portfolioExists) return new List<TransactionDto>();

                var transactions = await _context.Transactions
                    .Where(t => t.PortfolioId == portfolioId && t.Symbol == symbol.ToUpper())
                    .OrderByDescending(t => t.TransactionDate)
                    .ToListAsync();

                return await MapToTransactionDtos(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transactions by symbol");
                throw;
            }
        }

        private async Task<List<TransactionDto>> MapToTransactionDtos(IEnumerable<Transaction> transactions)
        {
            var symbols = transactions.Select(t => t.Symbol).Distinct().ToList();
            var stockInfos = new Dictionary<string, string>();

            // Get company names for all symbols
            foreach (var symbol in symbols)
            {
                var stockInfo = await _stockDataService.GetStockInfoAsync(symbol);
                stockInfos[symbol] = stockInfo?.CompanyName ?? symbol;
            }

            return transactions.Select(t => new TransactionDto
            {
                Id = t.Id,
                PortfolioId = t.PortfolioId,
                Symbol = t.Symbol,
                CompanyName = stockInfos.GetValueOrDefault(t.Symbol, t.Symbol),
                Type = t.Type,
                Quantity = t.Quantity,
                Price = t.Price,
                TotalAmount = t.TotalAmount,
                Fees = t.Fees,
                NetAmount = t.NetAmount,
                Notes = t.Notes,
                TransactionDate = t.TransactionDate,
                CreatedAt = t.CreatedAt
            }).ToList();
        }
    }
}