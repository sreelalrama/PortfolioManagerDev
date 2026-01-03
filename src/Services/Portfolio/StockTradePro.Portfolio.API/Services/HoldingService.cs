using Microsoft.EntityFrameworkCore;
using StockTradePro.Portfolio.API.Data;
using StockTradePro.Portfolio.API.Models.DTOs;
using StockTradePro.Portfolio.API.Models.Entities;

namespace StockTradePro.Portfolio.API.Services
{
    public class HoldingService : IHoldingService
    {
        private readonly PortfolioDbContext _context;
        private readonly IStockDataService _stockDataService;
        private readonly ILogger<HoldingService> _logger;

        public HoldingService(
            PortfolioDbContext context,
            IStockDataService stockDataService,
            ILogger<HoldingService> logger)
        {
            _context = context;
            _stockDataService = stockDataService;
            _logger = logger;
        }

        public async Task<List<HoldingDto>> GetPortfolioHoldingsAsync(int portfolioId, string userId)
        {
            try
            {
                // Verify user owns the portfolio
                var portfolioExists = await _context.Portfolios
                    .AnyAsync(p => p.Id == portfolioId && p.UserId == userId && p.IsActive);

                if (!portfolioExists) return new List<HoldingDto>();

                var holdings = await _context.Holdings
                    .Where(h => h.PortfolioId == portfolioId && h.Quantity > 0)
                    .OrderByDescending(h => h.CurrentValue)
                    .ToListAsync();

                // Get total portfolio value for percentage calculations
                var totalPortfolioValue = holdings.Sum(h => h.CurrentValue);

                return holdings.Select(h => new HoldingDto
                {
                    Id = h.Id,
                    PortfolioId = h.PortfolioId,
                    Symbol = h.Symbol,
                    CompanyName = h.CompanyName,
                    Quantity = h.Quantity,
                    AverageCost = h.AverageCost,
                    TotalCost = h.TotalCost,
                    CurrentPrice = h.CurrentPrice,
                    CurrentValue = h.CurrentValue,
                    UnrealizedGainLoss = h.UnrealizedGainLoss,
                    UnrealizedGainLossPercent = h.UnrealizedGainLossPercent,
                    LastUpdated = h.LastUpdated,
                    CreatedAt = h.CreatedAt,
                    PortfolioPercent = totalPortfolioValue > 0 ? (h.CurrentValue / totalPortfolioValue) * 100 : 0,
                    DayChange = 0, // Would need historical data
                    DayChangePercent = 0 // Would need historical data
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting portfolio holdings for portfolio: {PortfolioId}", portfolioId);
                throw;
            }
        }

        public async Task<HoldingDto?> GetHoldingBySymbolAsync(int portfolioId, string symbol, string userId)
        {
            try
            {
                // Verify user owns the portfolio
                var portfolioExists = await _context.Portfolios
                    .AnyAsync(p => p.Id == portfolioId && p.UserId == userId && p.IsActive);

                if (!portfolioExists) return null;

                var holding = await _context.Holdings
                    .FirstOrDefaultAsync(h => h.PortfolioId == portfolioId && h.Symbol == symbol.ToUpper() && h.Quantity > 0);

                if (holding == null) return null;

                return new HoldingDto
                {
                    Id = holding.Id,
                    PortfolioId = holding.PortfolioId,
                    Symbol = holding.Symbol,
                    CompanyName = holding.CompanyName,
                    Quantity = holding.Quantity,
                    AverageCost = holding.AverageCost,
                    TotalCost = holding.TotalCost,
                    CurrentPrice = holding.CurrentPrice,
                    CurrentValue = holding.CurrentValue,
                    UnrealizedGainLoss = holding.UnrealizedGainLoss,
                    UnrealizedGainLossPercent = holding.UnrealizedGainLossPercent,
                    LastUpdated = holding.LastUpdated,
                    CreatedAt = holding.CreatedAt,
                    PortfolioPercent = 0, // Would need portfolio total
                    DayChange = 0,
                    DayChangePercent = 0
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting holding by symbol: {Symbol} for portfolio: {PortfolioId}", symbol, portfolioId);
                throw;
            }
        }

        public async Task UpdateHoldingPricesAsync(int portfolioId)
        {
            try
            {
                var holdings = await _context.Holdings
                    .Where(h => h.PortfolioId == portfolioId && h.Quantity > 0)
                    .ToListAsync();

                if (!holdings.Any()) return;

                var symbols = holdings.Select(h => h.Symbol).ToList();
                var currentPrices = await _stockDataService.GetMultipleStockPricesAsync(symbols);

                foreach (var holding in holdings)
                {
                    if (currentPrices.TryGetValue(holding.Symbol, out var currentPrice))
                    {
                        holding.CurrentPrice = currentPrice;
                        holding.CurrentValue = holding.Quantity * currentPrice;
                        holding.UnrealizedGainLoss = holding.CurrentValue - holding.TotalCost;
                        holding.UnrealizedGainLossPercent = holding.TotalCost > 0
                            ? (holding.UnrealizedGainLoss / holding.TotalCost) * 100
                            : 0;
                        holding.LastUpdated = DateTime.UtcNow;
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating holding prices for portfolio: {PortfolioId}", portfolioId);
                throw;
            }
        }

        public async Task UpdateAllHoldingPricesAsync()
        {
            try
            {
                var portfolioIds = await _context.Holdings
                    .Where(h => h.Quantity > 0)
                    .Select(h => h.PortfolioId)
                    .Distinct()
                    .ToListAsync();

                foreach (var portfolioId in portfolioIds)
                {
                    await UpdateHoldingPricesAsync(portfolioId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating all holding prices");
                throw;
            }
        }

        public async Task RecalculateHoldingsAsync(int portfolioId)
        {
            try
            {
                // Get all transactions for this portfolio grouped by symbol
                var transactionsBySymbol = await _context.Transactions
                    .Where(t => t.PortfolioId == portfolioId)
                    .GroupBy(t => t.Symbol)
                    .ToListAsync();

                // Get existing holdings
                var existingHoldings = await _context.Holdings
                    .Where(h => h.PortfolioId == portfolioId)
                    .ToListAsync();

                foreach (var symbolGroup in transactionsBySymbol)
                {
                    var symbol = symbolGroup.Key;
                    var transactions = symbolGroup.OrderBy(t => t.TransactionDate).ToList();

                    // Calculate current position
                    var totalQuantity = 0;
                    var totalCost = 0m;
                    var totalShares = 0;

                    foreach (var transaction in transactions)
                    {
                        if (transaction.Type == "BUY")
                        {
                            totalQuantity += transaction.Quantity;
                            totalCost += transaction.NetAmount;
                            totalShares += transaction.Quantity;
                        }
                        else if (transaction.Type == "SELL")
                        {
                            totalQuantity -= transaction.Quantity;
                            // For FIFO cost basis calculation, we need more complex logic
                            // For simplicity, we'll calculate average cost across all shares
                            totalShares -= transaction.Quantity;
                        }
                    }

                    var existingHolding = existingHoldings.FirstOrDefault(h => h.Symbol == symbol);

                    if (totalQuantity > 0)
                    {
                        var averageCost = totalShares > 0 ? totalCost / totalShares : 0;
                        var adjustedTotalCost = totalQuantity * averageCost;

                        if (existingHolding == null)
                        {
                            // Get company name from stock data service
                            var stockInfo = await _stockDataService.GetStockInfoAsync(symbol);
                            var companyName = stockInfo?.CompanyName ?? symbol;

                            var newHolding = new Holding
                            {
                                PortfolioId = portfolioId,
                                Symbol = symbol,
                                CompanyName = companyName,
                                Quantity = totalQuantity,
                                AverageCost = averageCost,
                                TotalCost = adjustedTotalCost,
                                CurrentPrice = 0, // Will be updated by price update
                                CurrentValue = 0,
                                UnrealizedGainLoss = 0,
                                UnrealizedGainLossPercent = 0,
                                CreatedAt = DateTime.UtcNow,
                                LastUpdated = DateTime.UtcNow
                            };

                            _context.Holdings.Add(newHolding);
                        }
                        else
                        {
                            existingHolding.Quantity = totalQuantity;
                            existingHolding.AverageCost = averageCost;
                            existingHolding.TotalCost = adjustedTotalCost;
                            existingHolding.LastUpdated = DateTime.UtcNow;
                        }
                    }
                    else if (existingHolding != null)
                    {
                        // No shares left, remove holding
                        _context.Holdings.Remove(existingHolding);
                    }
                }

                await _context.SaveChangesAsync();

                // Update current prices for all holdings
                await UpdateHoldingPricesAsync(portfolioId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recalculating holdings for portfolio: {PortfolioId}", portfolioId);
                throw;
            }
        }
    }
}