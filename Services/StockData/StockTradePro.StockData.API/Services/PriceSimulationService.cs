using Microsoft.EntityFrameworkCore;
using StockTradePro.StockData.API.Data;
using StockTradePro.StockData.API.Models;
using StockTradePro.StockData.API.Models.DTOs;

namespace StockTradePro.StockData.API.Services
{
    public class PriceSimulationService : IPriceSimulationService
    {
        private readonly StockDataDbContext _context;
        private readonly ILogger<PriceSimulationService> _logger;
        private readonly Random _random;
        private readonly IConfiguration _configuration;

        // Market hours configuration
        private readonly TimeSpan _marketOpenTime;
        private readonly TimeSpan _marketCloseTime;
        private readonly TimeZoneInfo _marketTimeZone;

        public PriceSimulationService(
            StockDataDbContext context,
            ILogger<PriceSimulationService> logger,
            IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
            _random = new Random();

            // Configure market hours (9:30 AM to 4:00 PM EST)
            _marketOpenTime = new TimeSpan(
                _configuration.GetValue<int>("StockDataSettings:MarketOpenHour", 9),
                _configuration.GetValue<int>("StockDataSettings:MarketOpenMinute", 30),
                0);

            _marketCloseTime = new TimeSpan(
                _configuration.GetValue<int>("StockDataSettings:MarketCloseHour", 16),
                _configuration.GetValue<int>("StockDataSettings:MarketCloseMinute", 0),
                0);

            try
            {
                _marketTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            }
            catch
            {
                // Fallback for Linux/Mac
                _marketTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
            }
        }

        public async Task<bool> IsMarketOpenAsync()
        {
            var easternTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _marketTimeZone);
            var currentTime = easternTime.TimeOfDay;
            var currentDay = easternTime.DayOfWeek;

            // Market is closed on weekends
            if (currentDay == DayOfWeek.Saturday || currentDay == DayOfWeek.Sunday)
                return false;

            // Check if current time is within market hours
            return currentTime >= _marketOpenTime && currentTime <= _marketCloseTime;
        }

        public async Task UpdateAllStockPricesAsync()
        {
            try
            {
                var stocks = await _context.Stocks
                    .Include(s => s.StockPrices.Where(sp => sp.IsCurrentPrice))
                    .Where(s => s.IsActive)
                    .ToListAsync();

                var updatedPrices = new List<StockPrice>();
                var isMarketOpen = await IsMarketOpenAsync();

                foreach (var stock in stocks)
                {
                    var currentPrice = stock.StockPrices.FirstOrDefault(sp => sp.IsCurrentPrice);
                    if (currentPrice == null) continue;

                    var newPrice = GenerateNewPrice(stock, currentPrice, isMarketOpen);

                    // Mark old price as not current
                    currentPrice.IsCurrentPrice = false;

                    // Create new current price
                    var updatedPrice = new StockPrice
                    {
                        StockId = stock.Id,
                        Date = DateTime.UtcNow.Date,
                        OpenPrice = currentPrice.OpenPrice, // Keep today's open
                        HighPrice = Math.Max(currentPrice.HighPrice, newPrice),
                        LowPrice = Math.Min(currentPrice.LowPrice, newPrice),
                        ClosePrice = newPrice,
                        CurrentPrice = newPrice,
                        Volume = GenerateNewVolume(stock, currentPrice.Volume),
                        PriceChange = newPrice - currentPrice.OpenPrice,
                        LastUpdated = DateTime.UtcNow,
                        IsCurrentPrice = true
                    };

                    updatedPrice.PriceChangePercent = updatedPrice.OpenPrice != 0
                        ? (updatedPrice.PriceChange / updatedPrice.OpenPrice) * 100
                        : 0;

                    updatedPrices.Add(updatedPrice);
                }

                if (updatedPrices.Any())
                {
                    await _context.StockPrices.AddRangeAsync(updatedPrices);
                    await _context.SaveChangesAsync();

                    _logger.LogDebug("Updated prices for {Count} stocks", updatedPrices.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stock prices");
                throw;
            }
        }

        public async Task<StockPriceDto?> UpdateStockPriceAsync(string symbol)
        {
            try
            {
                var stock = await _context.Stocks
                    .Include(s => s.StockPrices.Where(sp => sp.IsCurrentPrice))
                    .FirstOrDefaultAsync(s => s.Symbol == symbol.ToUpper() && s.IsActive);

                if (stock == null) return null;

                var currentPrice = stock.StockPrices.FirstOrDefault(sp => sp.IsCurrentPrice);
                if (currentPrice == null) return null;

                var isMarketOpen = await IsMarketOpenAsync();
                var newPrice = GenerateNewPrice(stock, currentPrice, isMarketOpen);

                // Update current price
                currentPrice.CurrentPrice = newPrice;
                currentPrice.ClosePrice = newPrice;
                currentPrice.HighPrice = Math.Max(currentPrice.HighPrice, newPrice);
                currentPrice.LowPrice = Math.Min(currentPrice.LowPrice, newPrice);
                currentPrice.PriceChange = newPrice - currentPrice.OpenPrice;
                currentPrice.PriceChangePercent = currentPrice.OpenPrice != 0
                    ? (currentPrice.PriceChange / currentPrice.OpenPrice) * 100
                    : 0;
                currentPrice.LastUpdated = DateTime.UtcNow;
                currentPrice.Volume = GenerateNewVolume(stock, currentPrice.Volume);

                await _context.SaveChangesAsync();

                return new StockPriceDto
                {
                    Id = currentPrice.Id,
                    Symbol = stock.Symbol,
                    OpenPrice = currentPrice.OpenPrice,
                    HighPrice = currentPrice.HighPrice,
                    LowPrice = currentPrice.LowPrice,
                    ClosePrice = currentPrice.ClosePrice,
                    CurrentPrice = currentPrice.CurrentPrice,
                    Volume = currentPrice.Volume,
                    PriceChange = currentPrice.PriceChange,
                    PriceChangePercent = currentPrice.PriceChangePercent,
                    Date = currentPrice.Date,
                    LastUpdated = currentPrice.LastUpdated
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating price for stock: {Symbol}", symbol);
                return null;
            }
        }

        public async Task<List<StockDto>> GetUpdatedStocksAsync(DateTime since)
        {
            try
            {
                var stocks = await _context.Stocks
                    .Include(s => s.StockPrices.Where(sp => sp.IsCurrentPrice && sp.LastUpdated >= since))
                    .Where(s => s.IsActive && s.StockPrices.Any(sp => sp.IsCurrentPrice && sp.LastUpdated >= since))
                    .ToListAsync();

                return stocks.Select(s => MapToStockDto(s)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting updated stocks since: {Since}", since);
                return new List<StockDto>();
            }
        }

        public TimeSpan GetTimeUntilMarketOpen()
        {
            var easternTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _marketTimeZone);
            var marketOpen = easternTime.Date.Add(_marketOpenTime);

            // If it's past market open today, get next business day
            if (easternTime.TimeOfDay >= _marketOpenTime ||
                easternTime.DayOfWeek == DayOfWeek.Saturday ||
                easternTime.DayOfWeek == DayOfWeek.Sunday)
            {
                do
                {
                    marketOpen = marketOpen.AddDays(1);
                } while (marketOpen.DayOfWeek == DayOfWeek.Saturday || marketOpen.DayOfWeek == DayOfWeek.Sunday);
            }

            var utcMarketOpen = TimeZoneInfo.ConvertTimeToUtc(marketOpen, _marketTimeZone);
            return utcMarketOpen - DateTime.UtcNow;
        }

        public TimeSpan GetTimeUntilMarketClose()
        {
            var easternTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _marketTimeZone);
            var marketClose = easternTime.Date.Add(_marketCloseTime);

            if (easternTime.TimeOfDay >= _marketCloseTime)
            {
                marketClose = marketClose.AddDays(1);
            }

            var utcMarketClose = TimeZoneInfo.ConvertTimeToUtc(marketClose, _marketTimeZone);
            return utcMarketClose - DateTime.UtcNow;
        }

        private decimal GenerateNewPrice(Stock stock, StockPrice currentPrice, bool isMarketOpen)
        {
            var baseVolatility = GetStockVolatility(stock.Symbol);

            // Reduce volatility when market is closed
            var volatilityMultiplier = isMarketOpen ? 1.0 : 0.1;
            var actualVolatility = baseVolatility * volatilityMultiplier;

            // Generate random price movement
            var randomChange = (decimal)(_random.NextDouble() - 0.5) * 2; // -1 to 1
            var priceChangePercent = randomChange * (decimal)actualVolatility;

            // Apply maximum change limit
            var maxChangePercent = _configuration.GetValue<decimal>("StockDataSettings:MaxPriceChangePercent", 5.0m);
            priceChangePercent = Math.Max(-maxChangePercent, Math.Min(maxChangePercent, priceChangePercent));

            var newPrice = currentPrice.CurrentPrice * (1 + priceChangePercent / 100);

            // Ensure price doesn't go below $1
            return Math.Max(1.00m, Math.Round(newPrice, 2));
        }

        private long GenerateNewVolume(Stock stock, long currentVolume)
        {
            // Generate volume variation (±20%)
            var variation = (decimal)(_random.NextDouble() - 0.5) * 0.4m; // -0.2 to 0.2
            var newVolume = currentVolume * (1 + variation);

            return Math.Max(1000, (long)newVolume); // Minimum 1000 volume
        }

        private double GetStockVolatility(string symbol)
        {
            // Different volatility for different types of stocks
            return symbol switch
            {
                "TSLA" => 0.03,   // 3% - High volatility
                "NVDA" => 0.025,  // 2.5% - High volatility
                "AMD" => 0.025,   // 2.5% - High volatility
                "MRNA" => 0.04,   // 4% - Very high volatility
                "SPOT" => 0.03,   // 3% - High volatility
                "ZOOM" => 0.03,   // 3% - High volatility
                "NFLX" => 0.02,   // 2% - Medium-high volatility
                "META" => 0.02,   // 2% - Medium-high volatility
                "AAPL" => 0.015,  // 1.5% - Medium volatility
                "MSFT" => 0.015,  // 1.5% - Medium volatility
                "GOOGL" => 0.015, // 1.5% - Medium volatility
                "KO" => 0.005,    // 0.5% - Low volatility
                "PG" => 0.005,    // 0.5% - Low volatility
                "JNJ" => 0.008,   // 0.8% - Low volatility
                "WMT" => 0.008,   // 0.8% - Low volatility
                "BRK.A" => 0.005, // 0.5% - Very low volatility
                _ => 0.01         // 1% - Default volatility
            };
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