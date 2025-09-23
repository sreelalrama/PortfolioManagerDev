using StockTradePro.BlazorUI.Models.Common;
using StockTradePro.BlazorUI.Models.Stocks;
using StockTradePro.BlazorUI.Services;

namespace StockTradePro.BlazorUI.Services.Mocks
{
    public class MockStockDataService : IStockDataService
    {
        private readonly List<StockDto> _stocks;
        private readonly List<StockPriceDto> _priceHistory;
        private readonly List<MarketIndexDto> _marketIndices;
        private readonly Random _random;

        public MockStockDataService()
        {
            _random = new Random();
            _marketIndices = GenerateMarketIndices();
            _stocks = GenerateStockData();
            _priceHistory = GeneratePriceHistory();
        }

        private List<MarketIndexDto> GenerateMarketIndices()
        {
            return new List<MarketIndexDto>
            {
                new MarketIndexDto
                {
                    IndexName = "S&P 500",
                    IndexSymbol = "SPX",
                    CurrentValue = 4567.89,
                    Change = 23.45,
                    ChangePercent = 0.52,
                    LastUpdated = DateTime.UtcNow.AddMinutes(-5)
                },
                new MarketIndexDto
                {
                    IndexName = "Dow Jones",
                    IndexSymbol = "DJI",
                    CurrentValue = 35234.56,
                    Change = -89.12,
                    ChangePercent = -0.25,
                    LastUpdated = DateTime.UtcNow.AddMinutes(-5)
                },
                new MarketIndexDto
                {
                    IndexName = "NASDAQ",
                    IndexSymbol = "IXIC",
                    CurrentValue = 14567.23,
                    Change = 67.89,
                    ChangePercent = 0.47,
                    LastUpdated = DateTime.UtcNow.AddMinutes(-5)
                },
                new MarketIndexDto
                {
                    IndexName = "Russell 2000",
                    IndexSymbol = "RUT",
                    CurrentValue = 2123.45,
                    Change = 12.34,
                    ChangePercent = 0.58,
                    LastUpdated = DateTime.UtcNow.AddMinutes(-5)
                }
            };
        }

        private List<StockDto> GenerateStockData()
        {
            var stocks = new List<StockDto>
            {
                // Tech Stocks
                CreateStock(1, "AAPL", "Apple Inc.", "NASDAQ", "Technology", "Consumer Electronics", 2850000000000, 175.25, 2.45, 1.42, 45234567),
                CreateStock(2, "GOOGL", "Alphabet Inc.", "NASDAQ", "Technology", "Internet Software/Services", 1750000000000, 142.50, -1.25, -0.87, 23456789),
                CreateStock(3, "MSFT", "Microsoft Corporation", "NASDAQ", "Technology", "Computer Software", 2650000000000, 378.90, 5.67, 1.52, 34567890),
                CreateStock(4, "AMZN", "Amazon.com Inc.", "NASDAQ", "Consumer Cyclical", "Internet Retail", 1450000000000, 145.75, -2.33, -1.58, 56789012),
                CreateStock(5, "TSLA", "Tesla Inc.", "NASDAQ", "Consumer Cyclical", "Auto Manufacturers", 785000000000, 248.50, 12.45, 5.27, 78901234),
                CreateStock(6, "META", "Meta Platforms Inc.", "NASDAQ", "Technology", "Internet Software/Services", 825000000000, 325.80, -8.90, -2.66, 43210987),
                CreateStock(7, "NVDA", "NVIDIA Corporation", "NASDAQ", "Technology", "Semiconductors", 2150000000000, 875.25, 23.45, 2.76, 32109876),
                CreateStock(8, "NFLX", "Netflix Inc.", "NASDAQ", "Communication Services", "Entertainment", 198000000000, 445.60, -5.40, -1.20, 21098765),
                CreateStock(9, "AMD", "Advanced Micro Devices Inc.", "NASDAQ", "Technology", "Semiconductors", 192000000000, 118.75, 3.25, 2.81, 65432109),
                CreateStock(10, "INTC", "Intel Corporation", "NASDAQ", "Technology", "Semiconductors", 148000000000, 35.20, 0.85, 2.47, 54321098),

                // Financial Stocks
                CreateStock(11, "JPM", "JPMorgan Chase & Co.", "NYSE", "Financial Services", "Banks", 432000000000, 147.85, 2.15, 1.48, 12345678),
                CreateStock(12, "BAC", "Bank of America Corp.", "NYSE", "Financial Services", "Banks", 268000000000, 32.45, 0.67, 2.11, 23456789),
                CreateStock(13, "WFC", "Wells Fargo & Co.", "NYSE", "Financial Services", "Banks", 185000000000, 46.78, -0.89, -1.87, 34567890),
                CreateStock(14, "GS", "Goldman Sachs Group Inc.", "NYSE", "Financial Services", "Investment Banking", 125000000000, 368.90, 8.45, 2.34, 45678901),
                CreateStock(15, "V", "Visa Inc.", "NYSE", "Financial Services", "Credit Services", 485000000000, 226.75, 3.20, 1.43, 56789012),

                // Healthcare Stocks
                CreateStock(16, "JNJ", "Johnson & Johnson", "NYSE", "Healthcare", "Drug Manufacturers", 425000000000, 162.35, 1.45, 0.90, 23456789),
                CreateStock(17, "PFE", "Pfizer Inc.", "NYSE", "Healthcare", "Drug Manufacturers", 202000000000, 35.82, -0.55, -1.51, 34567890),
                CreateStock(18, "UNH", "UnitedHealth Group Inc.", "NYSE", "Healthcare", "Healthcare Plans", 485000000000, 512.90, 4.85, 0.96, 12345678),
                CreateStock(19, "ABBV", "AbbVie Inc.", "NYSE", "Healthcare", "Drug Manufacturers", 268000000000, 151.25, 2.35, 1.58, 45678901),
                CreateStock(20, "MRK", "Merck & Co. Inc.", "NYSE", "Healthcare", "Drug Manufacturers", 295000000000, 116.75, 0.95, 0.82, 56789012),

                // Energy Stocks
                CreateStock(21, "XOM", "Exxon Mobil Corp.", "NYSE", "Energy", "Oil & Gas Integrated", 425000000000, 102.45, -1.85, -1.77, 43210987),
                CreateStock(22, "CVX", "Chevron Corp.", "NYSE", "Energy", "Oil & Gas Integrated", 312000000000, 163.20, -2.45, -1.48, 32109876),
                CreateStock(23, "COP", "ConocoPhillips", "NYSE", "Energy", "Oil & Gas E&P", 145000000000, 112.85, -0.95, -0.83, 21098765),

                // Consumer Goods
                CreateStock(24, "KO", "Coca-Cola Co.", "NYSE", "Consumer Defensive", "Beverages", 268000000000, 61.75, 0.45, 0.73, 34567890),
                CreateStock(25, "PEP", "PepsiCo Inc.", "NASDAQ", "Consumer Defensive", "Beverages", 238000000000, 173.25, 1.85, 1.08, 23456789),
                CreateStock(26, "WMT", "Walmart Inc.", "NYSE", "Consumer Defensive", "Discount Stores", 485000000000, 162.45, 2.35, 1.47, 45678901),
                CreateStock(27, "PG", "Procter & Gamble Co.", "NYSE", "Consumer Defensive", "Household Products", 358000000000, 149.85, 0.95, 0.64, 56789012),

                // Industrial Stocks
                CreateStock(28, "BA", "Boeing Co.", "NYSE", "Industrials", "Aerospace & Defense", 125000000000, 206.75, -3.85, -1.83, 32109876),
                CreateStock(29, "CAT", "Caterpillar Inc.", "NYSE", "Industrials", "Farm & Heavy Construction", 142000000000, 267.45, 4.25, 1.61, 21098765),
                CreateStock(30, "GE", "General Electric Co.", "NYSE", "Industrials", "Conglomerates", 125000000000, 115.20, 2.45, 2.17, 43210987),

                // More diverse stocks
                CreateStock(31, "DIS", "Walt Disney Co.", "NYSE", "Communication Services", "Entertainment", 185000000000, 101.25, -1.45, -1.41, 34567890),
                CreateStock(32, "NKE", "Nike Inc.", "NYSE", "Consumer Cyclical", "Footwear & Accessories", 168000000000, 107.85, 2.15, 2.03, 23456789),
                CreateStock(33, "MCD", "McDonald's Corp.", "NYSE", "Consumer Cyclical", "Restaurants", 198000000000, 268.75, 1.85, 0.69, 12345678),
                CreateStock(34, "SBUX", "Starbucks Corp.", "NASDAQ", "Consumer Cyclical", "Restaurants", 125000000000, 108.45, -0.95, -0.87, 45678901),
                CreateStock(35, "HD", "Home Depot Inc.", "NYSE", "Consumer Cyclical", "Home Improvement Retail", 358000000000, 342.85, 5.25, 1.56, 56789012)
            };

            return stocks;
        }

        private StockDto CreateStock(int id, string symbol, string companyName, string exchange, string sector, string industry,
            double marketCap, double currentPrice, double priceChange, double priceChangePercent, long volume)
        {
            return new StockDto
            {
                Id = id,
                Symbol = symbol,
                CompanyName = companyName,
                Exchange = exchange,
                Sector = sector,
                Industry = industry,
                MarketCap = marketCap,
                Description = $"{companyName} operates in the {industry} industry within the {sector} sector.",
                Website = $"https://www.{symbol.ToLower()}.com",
                LogoUrl = $"https://logo.clearbit.com/{symbol.ToLower()}.com",
                IsActive = true,
                CurrentPrice = currentPrice,
                PriceChange = priceChange,
                PriceChangePercent = priceChangePercent,
                Volume = volume,
                LastUpdated = DateTime.UtcNow.AddMinutes(-_random.Next(1, 15))
            };
        }

        private List<StockPriceDto> GeneratePriceHistory()
        {
            var priceHistory = new List<StockPriceDto>();
            var symbols = new[] { "AAPL", "GOOGL", "MSFT", "AMZN", "TSLA", "META", "NVDA", "NFLX", "AMD", "INTC" };

            foreach (var symbol in symbols)
            {
                var stock = _stocks.First(s => s.Symbol == symbol);
                var basePrice = stock.CurrentPrice;
                var currentDate = DateTime.UtcNow.Date;

                // Generate 30 days of price history
                for (int i = 30; i >= 0; i--)
                {
                    var date = currentDate.AddDays(-i);
                    var dayVariation = (_random.NextDouble() - 0.5) * 0.1; // ±5% daily variation
                    var priceMultiplier = 1 + dayVariation;

                    var openPrice = basePrice * priceMultiplier;
                    var highVariation = _random.NextDouble() * 0.03; // Up to 3% higher
                    var lowVariation = _random.NextDouble() * 0.03; // Up to 3% lower

                    var highPrice = openPrice * (1 + highVariation);
                    var lowPrice = openPrice * (1 - lowVariation);
                    var closePrice = lowPrice + (_random.NextDouble() * (highPrice - lowPrice));

                    var volume = (long)(stock.Volume * (0.5 + _random.NextDouble()));
                    var priceChange = i == 0 ? stock.PriceChange : (closePrice - openPrice);
                    var priceChangePercent = openPrice > 0 ? (priceChange / openPrice) * 100 : 0;

                    priceHistory.Add(new StockPriceDto
                    {
                        Id = priceHistory.Count + 1,
                        Symbol = symbol,
                        OpenPrice = Math.Round(openPrice, 2),
                        HighPrice = Math.Round(highPrice, 2),
                        LowPrice = Math.Round(lowPrice, 2),
                        ClosePrice = Math.Round(closePrice, 2),
                        CurrentPrice = i == 0 ? stock.CurrentPrice : Math.Round(closePrice, 2),
                        Volume = volume,
                        PriceChange = Math.Round(priceChange, 2),
                        PriceChangePercent = Math.Round(priceChangePercent, 2),
                        Date = date,
                        LastUpdated = i == 0 ? DateTime.UtcNow : date.AddHours(16) // Market close time
                    });

                    basePrice = closePrice; // Use previous close as next base
                }
            }

            return priceHistory;
        }

        public async Task<MarketOverviewDto?> GetMarketOverviewAsync()
        {
            await Task.Delay(300); // Simulate API delay

            return new MarketOverviewDto
            {
                Indices = _marketIndices,
                TopGainers = await GetTopGainersAsync(5),
                TopLosers = await GetTopLosersAsync(5),
                MostActive = await GetMostActiveAsync(5),
                LastUpdated = DateTime.UtcNow
            };
        }

        public async Task<List<StockDto>> GetTrendingStocksAsync(int count = 10)
        {
            await Task.Delay(200); // Simulate API delay

            // Mix of high volume and significant price changes
            return _stocks
                .OrderByDescending(s => s.Volume * Math.Abs(s.PriceChangePercent))
                .Take(count)
                .ToList();
        }

        public async Task<List<StockDto>> GetTopGainersAsync(int count = 10)
        {
            await Task.Delay(150); // Simulate API delay

            return _stocks
                .Where(s => s.PriceChangePercent > 0)
                .OrderByDescending(s => s.PriceChangePercent)
                .Take(count)
                .ToList();
        }

        public async Task<List<StockDto>> GetTopLosersAsync(int count = 10)
        {
            await Task.Delay(150); // Simulate API delay

            return _stocks
                .Where(s => s.PriceChangePercent < 0)
                .OrderBy(s => s.PriceChangePercent)
                .Take(count)
                .ToList();
        }

        public async Task<List<StockDto>> GetMostActiveAsync(int count = 10)
        {
            await Task.Delay(150); // Simulate API delay

            return _stocks
                .OrderByDescending(s => s.Volume)
                .Take(count)
                .ToList();
        }

        public async Task<StockDto?> GetStockAsync(string symbol)
        {
            await Task.Delay(100); // Simulate API delay

            return _stocks.FirstOrDefault(s => s.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<PaginatedResult<StockDto>> GetStocksAsync(StockSearchDto searchDto)
        {
            await Task.Delay(250); // Simulate API delay

            var query = _stocks.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(searchDto.Query))
            {
                query = query.Where(s => s.Symbol.Contains(searchDto.Query, StringComparison.OrdinalIgnoreCase) ||
                                        s.CompanyName.Contains(searchDto.Query, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(searchDto.Sector))
            {
                query = query.Where(s => s.Sector.Equals(searchDto.Sector, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(searchDto.Exchange))
            {
                query = query.Where(s => s.Exchange.Equals(searchDto.Exchange, StringComparison.OrdinalIgnoreCase));
            }

            if (searchDto.MinPrice.HasValue)
            {
                query = query.Where(s => s.CurrentPrice >= searchDto.MinPrice.Value);
            }

            if (searchDto.MaxPrice.HasValue)
            {
                query = query.Where(s => s.CurrentPrice <= searchDto.MaxPrice.Value);
            }

            // Apply sorting
            query = searchDto.SortBy?.ToLower() switch
            {
                "symbol" => searchDto.SortOrder == "desc" ? query.OrderByDescending(s => s.Symbol) : query.OrderBy(s => s.Symbol),
                "name" => searchDto.SortOrder == "desc" ? query.OrderByDescending(s => s.CompanyName) : query.OrderBy(s => s.CompanyName),
                "price" => searchDto.SortOrder == "desc" ? query.OrderByDescending(s => s.CurrentPrice) : query.OrderBy(s => s.CurrentPrice),
                "change" => searchDto.SortOrder == "desc" ? query.OrderByDescending(s => s.PriceChangePercent) : query.OrderBy(s => s.PriceChangePercent),
                "volume" => searchDto.SortOrder == "desc" ? query.OrderByDescending(s => s.Volume) : query.OrderBy(s => s.Volume),
                "marketcap" => searchDto.SortOrder == "desc" ? query.OrderByDescending(s => s.MarketCap) : query.OrderBy(s => s.MarketCap),
                _ => query.OrderBy(s => s.Symbol)
            };

            var totalCount = query.Count();
            var stocks = query
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .ToList();

            return new PaginatedResult<StockDto>
            {
                Data = stocks,
                TotalRecords = totalCount,
                PageNumber = searchDto.Page,
                PageSize = searchDto.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / searchDto.PageSize)
            };
        }

        public async Task<PaginatedResult<StockDto>> SearchStocksAsync(string query, int page = 1, int pageSize = 20)
        {
            await Task.Delay(200); // Simulate API delay

            var searchDto = new StockSearchDto
            {
                Query = query,
                Page = page,
                PageSize = pageSize,
                SortBy = "symbol",
                SortOrder = "asc"
            };

            return await GetStocksAsync(searchDto);
        }

        public async Task<List<string>> GetSectorsAsync()
        {
            await Task.Delay(100); // Simulate API delay

            return _stocks
                .Select(s => s.Sector)
                .Distinct()
                .OrderBy(s => s)
                .ToList();
        }

        public async Task<List<string>> GetExchangesAsync()
        {
            await Task.Delay(100); // Simulate API delay

            return _stocks
                .Select(s => s.Exchange)
                .Distinct()
                .OrderBy(e => e)
                .ToList();
        }

        public async Task<List<StockPriceDto>> GetPricesAsync(string symbol, DateTime? fromDate = null, DateTime? toDate = null)
        {
            await Task.Delay(200); // Simulate API delay

            var query = _priceHistory.Where(p => p.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase));

            if (fromDate.HasValue)
            {
                query = query.Where(p => p.Date >= fromDate.Value.Date);
            }

            if (toDate.HasValue)
            {
                query = query.Where(p => p.Date <= toDate.Value.Date);
            }

            return query
                .OrderBy(p => p.Date)
                .ToList();
        }

        public async Task<StockPriceDto?> GetCurrentPriceAsync(string symbol)
        {
            await Task.Delay(100); // Simulate API delay

            return _priceHistory
                .Where(p => p.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(p => p.Date)
                .FirstOrDefault();
        }

        // Helper methods for testing
        public void UpdateStockPrice(string symbol, double newPrice)
        {
            var stock = _stocks.FirstOrDefault(s => s.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase));
            if (stock != null)
            {
                var oldPrice = stock.CurrentPrice;
                stock.CurrentPrice = newPrice;
                stock.PriceChange = newPrice - oldPrice;
                stock.PriceChangePercent = oldPrice > 0 ? (stock.PriceChange / oldPrice) * 100 : 0;
                stock.LastUpdated = DateTime.UtcNow;
            }
        }

        public void SimulateMarketMovement()
        {
            var random = new Random();

            // Update market indices
            foreach (var index in _marketIndices)
            {
                var changePercent = (random.NextDouble() - 0.5) * 2; // ±1%
                var change = index.CurrentValue * (changePercent / 100);
                index.CurrentValue += change;
                index.Change = change;
                index.ChangePercent = changePercent;
                index.LastUpdated = DateTime.UtcNow;
            }

            // Update stock prices
            foreach (var stock in _stocks)
            {
                var changePercent = (random.NextDouble() - 0.5) * 10; // ±5%
                var change = stock.CurrentPrice * (changePercent / 100);
                stock.CurrentPrice += change;
                stock.PriceChange = change;
                stock.PriceChangePercent = changePercent;
                stock.Volume = (long)(stock.Volume * (0.8 + random.NextDouble() * 0.4)); // ±20% volume change
                stock.LastUpdated = DateTime.UtcNow;
            }
        }
    }
}