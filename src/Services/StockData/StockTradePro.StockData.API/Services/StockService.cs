using Microsoft.EntityFrameworkCore;
using StockTradePro.StockData.API.Data;
using StockTradePro.StockData.API.Models;
using StockTradePro.StockData.API.Models.DTOs;

namespace StockTradePro.StockData.API.Services
{
    public class StockService : IStockService
    {
        private readonly StockDataDbContext _context;
        private readonly ILogger<StockService> _logger;

        public StockService(StockDataDbContext context, ILogger<StockService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PaginatedResult<StockDto>> GetStocksAsync(StockSearchDto searchDto)
        {
            try
            {
                var query = _context.Stocks
                    .Include(s => s.StockPrices.Where(sp => sp.IsCurrentPrice))
                    .Where(s => s.IsActive);

                // Apply filters
                if (!string.IsNullOrEmpty(searchDto.Query))
                {
                    query = query.Where(s => s.Symbol.Contains(searchDto.Query) ||
                                           s.CompanyName.Contains(searchDto.Query));
                }

                if (!string.IsNullOrEmpty(searchDto.Sector))
                {
                    query = query.Where(s => s.Sector == searchDto.Sector);
                }

                if (!string.IsNullOrEmpty(searchDto.Exchange))
                {
                    query = query.Where(s => s.Exchange == searchDto.Exchange);
                }

                // Apply price filters
                if (searchDto.MinPrice.HasValue)
                {
                    query = query.Where(s => s.StockPrices.Any(sp => sp.IsCurrentPrice && sp.CurrentPrice >= searchDto.MinPrice));
                }

                if (searchDto.MaxPrice.HasValue)
                {
                    query = query.Where(s => s.StockPrices.Any(sp => sp.IsCurrentPrice && sp.CurrentPrice <= searchDto.MaxPrice));
                }

                // Apply sorting
                query = searchDto.SortBy?.ToLower() switch
                {
                    "name" => searchDto.SortOrder == "desc" ? query.OrderByDescending(s => s.CompanyName) : query.OrderBy(s => s.CompanyName),
                    "price" => searchDto.SortOrder == "desc" ? query.OrderByDescending(s => s.StockPrices.Where(sp => sp.IsCurrentPrice).Select(sp => sp.CurrentPrice).FirstOrDefault()) : query.OrderBy(s => s.StockPrices.Where(sp => sp.IsCurrentPrice).Select(sp => sp.CurrentPrice).FirstOrDefault()),
                    "change" => searchDto.SortOrder == "desc" ? query.OrderByDescending(s => s.StockPrices.Where(sp => sp.IsCurrentPrice).Select(sp => sp.PriceChangePercent).FirstOrDefault()) : query.OrderBy(s => s.StockPrices.Where(sp => sp.IsCurrentPrice).Select(sp => sp.PriceChangePercent).FirstOrDefault()),
                    _ => searchDto.SortOrder == "desc" ? query.OrderByDescending(s => s.Symbol) : query.OrderBy(s => s.Symbol)
                };

                // Get total count
                var totalRecords = await query.CountAsync();

                // Apply pagination
                var stocks = await query
                    .Skip((searchDto.Page - 1) * searchDto.PageSize)
                    .Take(searchDto.PageSize)
                    .ToListAsync();

                // Map to DTOs
                var stockDtos = stocks.Select(s => MapToStockDto(s)).ToList();

                var totalPages = (int)Math.Ceiling((double)totalRecords / searchDto.PageSize);

                return new PaginatedResult<StockDto>
                {
                    Data = stockDtos,
                    TotalRecords = totalRecords,
                    PageNumber = searchDto.Page,
                    PageSize = searchDto.PageSize,
                    TotalPages = totalPages,
                    HasNextPage = searchDto.Page < totalPages,
                    HasPreviousPage = searchDto.Page > 1
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stocks with search criteria");
                throw;
            }
        }

        public async Task<StockDto?> GetStockBySymbolAsync(string symbol)
        {
            try
            {
                var stock = await _context.Stocks
                    .Include(s => s.StockPrices.Where(sp => sp.IsCurrentPrice))
                    .FirstOrDefaultAsync(s => s.Symbol == symbol.ToUpper() && s.IsActive);

                return stock != null ? MapToStockDto(stock) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock by symbol: {Symbol}", symbol);
                throw;
            }
        }

        public async Task<StockDto?> GetStockByIdAsync(int id)
        {
            try
            {
                var stock = await _context.Stocks
                    .Include(s => s.StockPrices.Where(sp => sp.IsCurrentPrice))
                    .FirstOrDefaultAsync(s => s.Id == id && s.IsActive);

                return stock != null ? MapToStockDto(stock) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock by ID: {Id}", id);
                throw;
            }
        }

        public async Task<List<StockPriceDto>> GetStockPricesAsync(string symbol, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var query = _context.StockPrices
                    .Include(sp => sp.Stock)
                    .Where(sp => sp.Stock.Symbol == symbol.ToUpper());

                if (fromDate.HasValue)
                {
                    query = query.Where(sp => sp.Date >= fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    query = query.Where(sp => sp.Date <= toDate.Value);
                }

                var prices = await query
                    .OrderBy(sp => sp.Date)
                    .ToListAsync();

                return prices.Select(p => new StockPriceDto
                {
                    Id = p.Id,
                    Symbol = p.Stock.Symbol,
                    OpenPrice = p.OpenPrice,
                    HighPrice = p.HighPrice,
                    LowPrice = p.LowPrice,
                    ClosePrice = p.ClosePrice,
                    CurrentPrice = p.CurrentPrice,
                    Volume = p.Volume,
                    PriceChange = p.PriceChange,
                    PriceChangePercent = p.PriceChangePercent,
                    Date = p.Date,
                    LastUpdated = p.LastUpdated
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stock prices for symbol: {Symbol}", symbol);
                throw;
            }
        }

        public async Task<List<StockDto>> GetTrendingStocksAsync(int count = 10)
        {
            try
            {
                var stocks = await _context.Stocks
                    .Include(s => s.StockPrices.Where(sp => sp.IsCurrentPrice))
                    .Where(s => s.IsActive)
                    .OrderByDescending(s => s.StockPrices.Where(sp => sp.IsCurrentPrice).Select(sp => sp.Volume).FirstOrDefault())
                    .Take(count)
                    .ToListAsync();

                return stocks.Select(s => MapToStockDto(s)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting trending stocks");
                throw;
            }
        }

        public async Task<List<StockDto>> GetTopGainersAsync(int count = 10)
        {
            try
            {
                var stocks = await _context.Stocks
                    .Include(s => s.StockPrices.Where(sp => sp.IsCurrentPrice))
                    .Where(s => s.IsActive)
                    .OrderByDescending(s => s.StockPrices.Where(sp => sp.IsCurrentPrice).Select(sp => sp.PriceChangePercent).FirstOrDefault())
                    .Take(count)
                    .ToListAsync();

                return stocks.Select(s => MapToStockDto(s)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top gainers");
                throw;
            }
        }

        public async Task<List<StockDto>> GetTopLosersAsync(int count = 10)
        {
            try
            {
                var stocks = await _context.Stocks
                    .Include(s => s.StockPrices.Where(sp => sp.IsCurrentPrice))
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.StockPrices.Where(sp => sp.IsCurrentPrice).Select(sp => sp.PriceChangePercent).FirstOrDefault())
                    .Take(count)
                    .ToListAsync();

                return stocks.Select(s => MapToStockDto(s)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top losers");
                throw;
            }
        }

        public async Task<List<StockDto>> GetMostActiveAsync(int count = 10)
        {
            try
            {
                var stocks = await _context.Stocks
                    .Include(s => s.StockPrices.Where(sp => sp.IsCurrentPrice))
                    .Where(s => s.IsActive)
                    .OrderByDescending(s => s.StockPrices.Where(sp => sp.IsCurrentPrice).Select(sp => sp.Volume).FirstOrDefault())
                    .Take(count)
                    .ToListAsync();

                return stocks.Select(s => MapToStockDto(s)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting most active stocks");
                throw;
            }
        }

        public async Task<MarketOverviewDto> GetMarketOverviewAsync()
        {
            try
            {
                var indices = await _context.MarketData
                    .Where(m => m.IsActive)
                    .Select(m => new MarketIndexDto
                    {
                        IndexName = m.IndexName,
                        IndexSymbol = m.IndexSymbol,
                        CurrentValue = m.CurrentValue,
                        Change = m.Change,
                        ChangePercent = m.ChangePercent,
                        LastUpdated = m.LastUpdated
                    })
                    .ToListAsync();

                var topGainers = await GetTopGainersAsync(5);
                var topLosers = await GetTopLosersAsync(5);
                var mostActive = await GetMostActiveAsync(5);

                return new MarketOverviewDto
                {
                    Indices = indices,
                    TopGainers = topGainers,
                    TopLosers = topLosers,
                    MostActive = mostActive,
                    LastUpdated = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting market overview");
                throw;
            }
        }

        public async Task<List<string>> GetSectorsAsync()
        {
            try
            {
                return await _context.Stocks
                    .Where(s => s.IsActive && !string.IsNullOrEmpty(s.Sector))
                    .Select(s => s.Sector)
                    .Distinct()
                    .OrderBy(s => s)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sectors");
                throw;
            }
        }

        public async Task<List<string>> GetExchangesAsync()
        {
            try
            {
                return await _context.Stocks
                    .Where(s => s.IsActive && !string.IsNullOrEmpty(s.Exchange))
                    .Select(s => s.Exchange)
                    .Distinct()
                    .OrderBy(s => s)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting exchanges");
                throw;
            }
        }

        private static StockDto MapToStockDto(Stock stock)
        {
            var currentPrice = stock.StockPrices.FirstOrDefault(sp => sp.IsCurrentPrice);

            return new StockDto
            {
                Id = stock.Id,
                Symbol = stock.Symbol,
                CompanyName = stock.CompanyName,
                Exchange = stock.Exchange,
                Sector = stock.Sector,
                Industry = stock.Industry,
                MarketCap = stock.MarketCap,
                Description = stock.Description,
                Website = stock.Website,
                LogoUrl = stock.LogoUrl,
                IsActive = stock.IsActive,
                CurrentPrice = currentPrice?.CurrentPrice ?? 0,
                PriceChange = currentPrice?.PriceChange ?? 0,
                PriceChangePercent = currentPrice?.PriceChangePercent ?? 0,
                Volume = currentPrice?.Volume ?? 0,
                LastUpdated = currentPrice?.LastUpdated ?? DateTime.UtcNow
            };
        }
    }
}