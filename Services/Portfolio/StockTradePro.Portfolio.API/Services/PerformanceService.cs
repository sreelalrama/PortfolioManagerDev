using Microsoft.EntityFrameworkCore;
using StockTradePro.Portfolio.API.Data;
using StockTradePro.Portfolio.API.Models.DTOs;
using StockTradePro.Portfolio.API.Models.Entities;

namespace StockTradePro.Portfolio.API.Services
{
    public class PerformanceService : IPerformanceService
    {
        private readonly PortfolioDbContext _context;
        private readonly IHoldingService _holdingService;
        private readonly ILogger<PerformanceService> _logger;

        public PerformanceService(
            PortfolioDbContext context,
            IHoldingService holdingService,
            ILogger<PerformanceService> logger)
        {
            _context = context;
            _holdingService = holdingService;
            _logger = logger;
        }

        public async Task<List<PortfolioPerformanceDto>> GetPortfolioPerformanceAsync(
            int portfolioId, string userId, int days = 30)
        {
            try
            {
                // Verify user owns the portfolio
                var portfolioExists = await _context.Portfolios
                    .AnyAsync(p => p.Id == portfolioId && p.UserId == userId && p.IsActive);

                if (!portfolioExists) return new List<PortfolioPerformanceDto>();

                var fromDate = DateTime.UtcNow.AddDays(-days).Date;

                var performances = await _context.PortfolioPerformances
                    .Where(p => p.PortfolioId == portfolioId && p.Date >= fromDate)
                    .OrderBy(p => p.Date)
                    .Select(p => new PortfolioPerformanceDto
                    {
                        Id = p.Id,
                        PortfolioId = p.PortfolioId,
                        Date = p.Date,
                        TotalValue = p.TotalValue,
                        DayChange = p.DayChange,
                        DayChangePercent = p.DayChangePercent,
                        TotalReturn = p.TotalReturn,
                        TotalReturnPercent = p.TotalReturnPercent,
                        TotalHoldings = p.TotalHoldings,
                        CashValue = p.CashValue,
                        MarketValue = p.MarketValue,
                        CreatedAt = p.CreatedAt
                    })
                    .ToListAsync();

                return performances;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting portfolio performance for portfolio: {PortfolioId}", portfolioId);
                throw;
            }
        }

        public async Task CalculateAndSavePerformanceAsync(int portfolioId)
        {
            try
            {
                var portfolio = await _context.Portfolios
                    .Include(p => p.Holdings.Where(h => h.Quantity > 0))
                    .FirstOrDefaultAsync(p => p.Id == portfolioId && p.IsActive);

                if (portfolio == null) return;

                var today = DateTime.UtcNow.Date;

                // Check if performance already calculated for today
                var existingPerformance = await _context.PortfolioPerformances
                    .FirstOrDefaultAsync(p => p.PortfolioId == portfolioId && p.Date == today);

                // Update holding prices first
                await _holdingService.UpdateHoldingPricesAsync(portfolioId);

                // Recalculate portfolio totals
                var totalMarketValue = portfolio.Holdings
                    .Where(h => h.Quantity > 0)
                    .Sum(h => h.CurrentValue);

                var totalCost = portfolio.Holdings
                    .Where(h => h.Quantity > 0)
                    .Sum(h => h.TotalCost);

                // Get previous day's performance for day change calculation
                var previousPerformance = await _context.PortfolioPerformances
                    .Where(p => p.PortfolioId == portfolioId && p.Date < today)
                    .OrderByDescending(p => p.Date)
                    .FirstOrDefaultAsync();

                var previousValue = previousPerformance?.TotalValue ?? portfolio.InitialValue;
                var dayChange = totalMarketValue - previousValue;
                var dayChangePercent = previousValue > 0 ? (dayChange / previousValue) * 100 : 0;

                // Calculate total return
                var totalReturn = totalMarketValue - totalCost;
                var totalReturnPercent = totalCost > 0 ? (totalReturn / totalCost) * 100 : 0;

                if (existingPerformance != null)
                {
                    // Update existing performance
                    existingPerformance.TotalValue = totalMarketValue;
                    existingPerformance.DayChange = dayChange;
                    existingPerformance.DayChangePercent = dayChangePercent;
                    existingPerformance.TotalReturn = totalReturn;
                    existingPerformance.TotalReturnPercent = totalReturnPercent;
                    existingPerformance.TotalHoldings = portfolio.Holdings.Count(h => h.Quantity > 0);
                    existingPerformance.CashValue = 0; // Could be enhanced to track cash
                    existingPerformance.MarketValue = totalMarketValue;
                }
                else
                {
                    // Create new performance record
                    var newPerformance = new PortfolioPerformance
                    {
                        PortfolioId = portfolioId,
                        Date = today,
                        TotalValue = totalMarketValue,
                        DayChange = dayChange,
                        DayChangePercent = dayChangePercent,
                        TotalReturn = totalReturn,
                        TotalReturnPercent = totalReturnPercent,
                        TotalHoldings = portfolio.Holdings.Count(h => h.Quantity > 0),
                        CashValue = 0, // Could be enhanced to track cash
                        MarketValue = totalMarketValue,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.PortfolioPerformances.Add(newPerformance);
                }

                // Update portfolio summary
                portfolio.CurrentValue = totalMarketValue;
                portfolio.TotalGainLoss = totalReturn;
                portfolio.TotalGainLossPercent = totalReturnPercent;
                portfolio.LastCalculatedAt = DateTime.UtcNow;
                portfolio.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating performance for portfolio: {PortfolioId}", portfolioId);
                throw;
            }
        }

        public async Task CalculateAllPortfolioPerformancesAsync()
        {
            try
            {
                var activePortfolioIds = await _context.Portfolios
                    .Where(p => p.IsActive)
                    .Select(p => p.Id)
                    .ToListAsync();

                var tasks = activePortfolioIds.Select(async portfolioId =>
                {
                    try
                    {
                        await CalculateAndSavePerformanceAsync(portfolioId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error calculating performance for portfolio: {PortfolioId}", portfolioId);
                    }
                });

                await Task.WhenAll(tasks);

                _logger.LogInformation("Calculated performance for {Count} portfolios", activePortfolioIds.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating all portfolio performances");
                throw;
            }
        }

        public async Task<Dictionary<string, decimal>> GetPortfolioMetricsAsync(int portfolioId, string userId)
        {
            try
            {
                // Verify user owns the portfolio
                var portfolio = await _context.Portfolios
                    .Include(p => p.Holdings.Where(h => h.Quantity > 0))
                    .Include(p => p.Transactions)
                    .FirstOrDefaultAsync(p => p.Id == portfolioId && p.UserId == userId && p.IsActive);

                if (portfolio == null) return new Dictionary<string, decimal>();

                var metrics = new Dictionary<string, decimal>();

                // Basic metrics
                metrics["CurrentValue"] = portfolio.CurrentValue;
                metrics["TotalCost"] = portfolio.Holdings.Where(h => h.Quantity > 0).Sum(h => h.TotalCost);
                metrics["TotalGainLoss"] = portfolio.TotalGainLoss;
                metrics["TotalGainLossPercent"] = portfolio.TotalGainLossPercent;
                metrics["TotalHoldings"] = portfolio.Holdings.Count(h => h.Quantity > 0);

                // Transaction metrics
                var transactions = portfolio.Transactions.OrderBy(t => t.TransactionDate).ToList();
                metrics["TotalTransactions"] = transactions.Count;
                metrics["TotalBuyTransactions"] = transactions.Count(t => t.Type == "BUY");
                metrics["TotalSellTransactions"] = transactions.Count(t => t.Type == "SELL");
                metrics["TotalFesPaid"] = transactions.Sum(t => t.Fees);

                // Performance metrics (last 30 days)
                var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
                var recentPerformances = await _context.PortfolioPerformances
                    .Where(p => p.PortfolioId == portfolioId && p.Date >= thirtyDaysAgo)
                    .OrderBy(p => p.Date)
                    .ToListAsync();

                if (recentPerformances.Any())
                {
                    var oldestValue = recentPerformances.First().TotalValue;
                    var currentValue = recentPerformances.Last().TotalValue;

                    metrics["ThirtyDayReturn"] = currentValue - oldestValue;
                    metrics["ThirtyDayReturnPercent"] = oldestValue > 0 ? ((currentValue - oldestValue) / oldestValue) * 100 : 0;

                    // Volatility (standard deviation of daily returns)
                    var dailyReturns = new List<decimal>();
                    for (int i = 1; i < recentPerformances.Count; i++)
                    {
                        var previousValue = recentPerformances[i - 1].TotalValue;
                        var currentValueDay = recentPerformances[i].TotalValue;
                        if (previousValue > 0)
                        {
                            dailyReturns.Add(((currentValueDay - previousValue) / previousValue) * 100);
                        }
                    }

                    if (dailyReturns.Any())
                    {
                        var meanReturn = dailyReturns.Average();
                        var variance = dailyReturns.Select(r => (r - meanReturn) * (r - meanReturn)).Average();
                        metrics["Volatility"] = (decimal)Math.Sqrt((double)variance);
                    }
                    else
                    {
                        metrics["Volatility"] = 0;
                    }
                }
                else
                {
                    metrics["ThirtyDayReturn"] = 0;
                    metrics["ThirtyDayReturnPercent"] = 0;
                    metrics["Volatility"] = 0;
                }

                // Best and worst performing holdings
                var holdings = portfolio.Holdings.Where(h => h.Quantity > 0).ToList();
                if (holdings.Any())
                {
                    metrics["BestPerformingHoldingPercent"] = holdings.Max(h => h.UnrealizedGainLossPercent);
                    metrics["WorstPerformingHoldingPercent"] = holdings.Min(h => h.UnrealizedGainLossPercent);

                    // Diversity metrics
                    metrics["AverageHoldingValue"] = holdings.Average(h => h.CurrentValue);
                    metrics["LargestHoldingPercent"] = holdings.Any() && portfolio.CurrentValue > 0
                        ? (holdings.Max(h => h.CurrentValue) / portfolio.CurrentValue) * 100
                        : 0;
                }
                else
                {
                    metrics["BestPerformingHoldingPercent"] = 0;
                    metrics["WorstPerformingHoldingPercent"] = 0;
                    metrics["AverageHoldingValue"] = 0;
                    metrics["LargestHoldingPercent"] = 0;
                }

                return metrics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting portfolio metrics for portfolio: {PortfolioId}", portfolioId);
                throw;
            }
        }
    }
}