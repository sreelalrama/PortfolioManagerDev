using Microsoft.EntityFrameworkCore;
using StockTradePro.Portfolio.API.Data;
using StockTradePro.Portfolio.API.Models.DTOs;
using StockTradePro.Portfolio.API.Models.Entities;

namespace StockTradePro.Portfolio.API.Services
{
    public class PortfolioService : IPortfolioService
    {
        private readonly PortfolioDbContext _context;
        private readonly IHoldingService _holdingService;
        private readonly ILogger<PortfolioService> _logger;

        public PortfolioService(
            PortfolioDbContext context,
            IHoldingService holdingService,
            ILogger<PortfolioService> logger)
        {
            _context = context;
            _holdingService = holdingService;
            _logger = logger;
        }

        public async Task<PaginatedResult<PortfolioSummaryDto>> GetUserPortfoliosAsync(string userId, int page = 1, int pageSize = 10)
        {
            try
            {
                var query = _context.Portfolios
                    .Where(p => p.UserId == userId && p.IsActive)
                    .OrderByDescending(p => p.UpdatedAt);

                var totalRecords = await query.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                var portfolios = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new PortfolioSummaryDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Type = p.Type,
                        CurrentValue = p.CurrentValue,
                        TotalGainLoss = p.TotalGainLoss,
                        TotalGainLossPercent = p.TotalGainLossPercent,
                        TotalHoldings = p.Holdings.Count(h => h.Quantity > 0),
                        LastCalculatedAt = p.LastCalculatedAt ?? p.UpdatedAt,
                        CreatedAt = p.CreatedAt
                    })
                    .ToListAsync();

                return new PaginatedResult<PortfolioSummaryDto>
                {
                    Data = portfolios,
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
                _logger.LogError(ex, "Error getting user portfolios for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<PortfolioDto?> GetPortfolioByIdAsync(int portfolioId, string userId)
        {
            try
            {
                var portfolio = await _context.Portfolios
                    .Include(p => p.Holdings.Where(h => h.Quantity > 0))
                    .Include(p => p.Transactions)
                    .FirstOrDefaultAsync(p => p.Id == portfolioId && p.UserId == userId && p.IsActive);

                if (portfolio == null) return null;

                var holdings = await _holdingService.GetPortfolioHoldingsAsync(portfolioId, userId);

                return new PortfolioDto
                {
                    Id = portfolio.Id,
                    UserId = portfolio.UserId,
                    Name = portfolio.Name,
                    Description = portfolio.Description,
                    Type = portfolio.Type,
                    InitialValue = portfolio.InitialValue,
                    CurrentValue = portfolio.CurrentValue,
                    TotalGainLoss = portfolio.TotalGainLoss,
                    TotalGainLossPercent = portfolio.TotalGainLossPercent,
                    IsActive = portfolio.IsActive,
                    IsPublic = portfolio.IsPublic,
                    CreatedAt = portfolio.CreatedAt,
                    UpdatedAt = portfolio.UpdatedAt,
                    LastCalculatedAt = portfolio.LastCalculatedAt,
                    TotalHoldings = holdings.Count,
                    TotalTransactions = portfolio.Transactions.Count,
                    Holdings = holdings
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting portfolio by ID: {PortfolioId} for user: {UserId}", portfolioId, userId);
                throw;
            }
        }

        public async Task<PortfolioDto> CreatePortfolioAsync(string userId, CreatePortfolioDto createPortfolioDto)
        {
            try
            {
                // Check user portfolio limit
                var userPortfolioCount = await _context.Portfolios
                    .CountAsync(p => p.UserId == userId && p.IsActive);

                const int maxPortfolios = 10; // Could be moved to configuration
                if (userPortfolioCount >= maxPortfolios)
                {
                    throw new InvalidOperationException($"Maximum number of portfolios ({maxPortfolios}) reached");
                }

                var portfolio = new Models.Entities.Portfolio
                {
                    UserId = userId,
                    Name = createPortfolioDto.Name,
                    Description = createPortfolioDto.Description,
                    Type = createPortfolioDto.Type,
                    InitialValue = createPortfolioDto.InitialValue,
                    CurrentValue = createPortfolioDto.InitialValue,
                    IsPublic = createPortfolioDto.IsPublic,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Portfolios.Add(portfolio);
                await _context.SaveChangesAsync();

                return new PortfolioDto
                {
                    Id = portfolio.Id,
                    UserId = portfolio.UserId,
                    Name = portfolio.Name,
                    Description = portfolio.Description,
                    Type = portfolio.Type,
                    InitialValue = portfolio.InitialValue,
                    CurrentValue = portfolio.CurrentValue,
                    TotalGainLoss = 0,
                    TotalGainLossPercent = 0,
                    IsActive = portfolio.IsActive,
                    IsPublic = portfolio.IsPublic,
                    CreatedAt = portfolio.CreatedAt,
                    UpdatedAt = portfolio.UpdatedAt,
                    TotalHoldings = 0,
                    TotalTransactions = 0,
                    Holdings = new List<HoldingDto>()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating portfolio for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<PortfolioDto?> UpdatePortfolioAsync(int portfolioId, string userId, UpdatePortfolioDto updatePortfolioDto)
        {
            try
            {
                var portfolio = await _context.Portfolios
                    .FirstOrDefaultAsync(p => p.Id == portfolioId && p.UserId == userId && p.IsActive);

                if (portfolio == null) return null;

                portfolio.Name = updatePortfolioDto.Name;
                portfolio.Description = updatePortfolioDto.Description;
                portfolio.Type = updatePortfolioDto.Type;
                portfolio.IsPublic = updatePortfolioDto.IsPublic;
                portfolio.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return await GetPortfolioByIdAsync(portfolioId, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating portfolio: {PortfolioId} for user: {UserId}", portfolioId, userId);
                throw;
            }
        }

        public async Task<bool> DeletePortfolioAsync(int portfolioId, string userId)
        {
            try
            {
                var portfolio = await _context.Portfolios
                    .FirstOrDefaultAsync(p => p.Id == portfolioId && p.UserId == userId && p.IsActive);

                if (portfolio == null) return false;

                // Soft delete
                portfolio.IsActive = false;
                portfolio.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting portfolio: {PortfolioId} for user: {UserId}", portfolioId, userId);
                throw;
            }
        }

        public async Task<bool> PortfolioExistsAsync(int portfolioId, string userId)
        {
            return await _context.Portfolios
                .AnyAsync(p => p.Id == portfolioId && p.UserId == userId && p.IsActive);
        }

        public async Task<List<PortfolioSummaryDto>> GetPublicPortfoliosAsync(int page = 1, int pageSize = 20)
        {
            try
            {
                return await _context.Portfolios
                    .Where(p => p.IsPublic && p.IsActive)
                    .OrderByDescending(p => p.TotalGainLossPercent)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new PortfolioSummaryDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Type = p.Type,
                        CurrentValue = p.CurrentValue,
                        TotalGainLoss = p.TotalGainLoss,
                        TotalGainLossPercent = p.TotalGainLossPercent,
                        TotalHoldings = p.Holdings.Count(h => h.Quantity > 0),
                        LastCalculatedAt = p.LastCalculatedAt ?? p.UpdatedAt,
                        CreatedAt = p.CreatedAt
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting public portfolios");
                throw;
            }
        }

        public async Task RecalculatePortfolioValueAsync(int portfolioId)
        {
            try
            {
                var portfolio = await _context.Portfolios
                    .Include(p => p.Holdings.Where(h => h.Quantity > 0))
                    .FirstOrDefaultAsync(p => p.Id == portfolioId && p.IsActive);

                if (portfolio == null) return;

                // Update holding prices first
                await _holdingService.UpdateHoldingPricesAsync(portfolioId);

                // Recalculate portfolio totals
                var totalCurrentValue = portfolio.Holdings
                    .Where(h => h.Quantity > 0)
                    .Sum(h => h.CurrentValue);

                var totalCost = portfolio.Holdings
                    .Where(h => h.Quantity > 0)
                    .Sum(h => h.TotalCost);

                portfolio.CurrentValue = totalCurrentValue;
                portfolio.TotalGainLoss = totalCurrentValue - totalCost;
                portfolio.TotalGainLossPercent = totalCost > 0
                    ? (portfolio.TotalGainLoss / totalCost) * 100
                    : 0;

                portfolio.LastCalculatedAt = DateTime.UtcNow;
                portfolio.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recalculating portfolio value: {PortfolioId}", portfolioId);
                throw;
            }
        }
    }
}