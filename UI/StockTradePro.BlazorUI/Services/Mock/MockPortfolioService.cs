using StockTradePro.BlazorUI.Models.Common;
using StockTradePro.BlazorUI.Models.Portfolio;
using StockTradePro.BlazorUI.Services;

namespace StockTradePro.BlazorUI.Services.Mocks
{
    public class MockPortfolioService : IPortfolioService
    {
        private readonly List<PortfolioDto> _portfolios;
        private readonly List<TransactionDto> _transactions;
        private readonly Dictionary<string, double> _stockPrices;
        private readonly Dictionary<string, string> _companyNames;
        private int _nextPortfolioId;
        private int _nextTransactionId;
        private string _currentUserId = "1"; // Default to user "1" for testing

        public MockPortfolioService()
        {
            _nextPortfolioId = 1;
            _nextTransactionId = 1;
            _stockPrices = GenerateStockPrices();
            _companyNames = GenerateCompanyNames();
            _transactions = GenerateMockTransactions();
            _portfolios = GenerateMockPortfolios();
        }

        private Dictionary<string, double> GenerateStockPrices()
        {
            return new Dictionary<string, double>
            {
                { "AAPL", 175.25 },
                { "GOOGL", 142.50 },
                { "MSFT", 378.90 },
                { "AMZN", 145.75 },
                { "TSLA", 248.50 },
                { "META", 325.80 },
                { "NVDA", 875.25 },
                { "NFLX", 445.60 },
                { "AMD", 118.75 },
                { "INTC", 35.20 }
            };
        }

        private Dictionary<string, string> GenerateCompanyNames()
        {
            return new Dictionary<string, string>
            {
                { "AAPL", "Apple Inc." },
                { "GOOGL", "Alphabet Inc." },
                { "MSFT", "Microsoft Corporation" },
                { "AMZN", "Amazon.com Inc." },
                { "TSLA", "Tesla Inc." },
                { "META", "Meta Platforms Inc." },
                { "NVDA", "NVIDIA Corporation" },
                { "NFLX", "Netflix Inc." },
                { "AMD", "Advanced Micro Devices Inc." },
                { "INTC", "Intel Corporation" }
            };
        }

        private List<TransactionDto> GenerateMockTransactions()
        {
            var transactions = new List<TransactionDto>();

            // Portfolio 1 transactions
            transactions.AddRange(new[]
            {
                CreateTransaction(1, "AAPL", "BUY", 50, 150.00, 9.99, DateTime.UtcNow.AddDays(-30), "Initial purchase"),
                CreateTransaction(1, "GOOGL", "BUY", 25, 140.00, 9.99, DateTime.UtcNow.AddDays(-25), "Tech diversification"),
                CreateTransaction(1, "AAPL", "BUY", 25, 160.00, 9.99, DateTime.UtcNow.AddDays(-20), "Adding to position"),
                CreateTransaction(1, "MSFT", "BUY", 30, 350.00, 9.99, DateTime.UtcNow.AddDays(-15), "Cloud play"),
                CreateTransaction(1, "AAPL", "SELL", 10, 170.00, 9.99, DateTime.UtcNow.AddDays(-5), "Taking profits")
            });

            // Portfolio 2 transactions
            transactions.AddRange(new[]
            {
                CreateTransaction(2, "TSLA", "BUY", 40, 220.00, 9.99, DateTime.UtcNow.AddDays(-45), "EV investment"),
                CreateTransaction(2, "NVDA", "BUY", 10, 800.00, 9.99, DateTime.UtcNow.AddDays(-40), "AI boom"),
                CreateTransaction(2, "AMD", "BUY", 75, 100.00, 9.99, DateTime.UtcNow.AddDays(-35), "Chip competition"),
                CreateTransaction(2, "TSLA", "BUY", 20, 240.00, 9.99, DateTime.UtcNow.AddDays(-10), "Doubling down")
            });

            // Portfolio 3 transactions (Public portfolio)
            transactions.AddRange(new[]
            {
                CreateTransaction(3, "META", "BUY", 30, 300.00, 9.99, DateTime.UtcNow.AddDays(-60), "Social media bet"),
                CreateTransaction(3, "NFLX", "BUY", 20, 400.00, 9.99, DateTime.UtcNow.AddDays(-55), "Streaming leader"),
                CreateTransaction(3, "AMZN", "BUY", 35, 130.00, 9.99, DateTime.UtcNow.AddDays(-50), "E-commerce giant")
            });

            return transactions;
        }

        private TransactionDto CreateTransaction(int portfolioId, string symbol, string type, int quantity, double price, double fees, DateTime date, string notes)
        {
            var totalAmount = quantity * price;
            var netAmount = type == "BUY" ? totalAmount + fees : totalAmount - fees;

            return new TransactionDto
            {
                Id = _nextTransactionId++,
                PortfolioId = portfolioId,
                Symbol = symbol,
                CompanyName = _companyNames.GetValueOrDefault(symbol, symbol),
                Type = type,
                Quantity = quantity,
                Price = price,
                TotalAmount = totalAmount,
                Fees = fees,
                NetAmount = netAmount,
                Notes = notes,
                TransactionDate = date,
                CreatedAt = date
            };
        }

        private List<PortfolioDto> GenerateMockPortfolios()
        {
            var portfolios = new List<PortfolioDto>
            {
                new PortfolioDto
                {
                    Id = 1,
                    UserId = "1",
                    Name = "Growth Portfolio",
                    Description = "Long-term growth focused on tech stocks",
                    Type = "Growth",
                    InitialValue = 50000.00,
                    IsActive = true,
                    IsPublic = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    UpdatedAt = DateTime.UtcNow.AddDays(-1),
                    LastCalculatedAt = DateTime.UtcNow.AddHours(-1)
                },
                new PortfolioDto
                {
                    Id = 2,
                    UserId = "1",
                    Name = "Tech Innovation",
                    Description = "High-risk, high-reward tech investments",
                    Type = "Aggressive",
                    InitialValue = 25000.00,
                    IsActive = true,
                    IsPublic = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-45),
                    UpdatedAt = DateTime.UtcNow.AddDays(-2),
                    LastCalculatedAt = DateTime.UtcNow.AddHours(-2)
                },
                new PortfolioDto
                {
                    Id = 3,
                    UserId = "2",
                    Name = "FAANG Strategy",
                    Description = "Investing in major tech companies",
                    Type = "Blue Chip",
                    InitialValue = 75000.00,
                    IsActive = true,
                    IsPublic = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-60),
                    UpdatedAt = DateTime.UtcNow.AddDays(-3),
                    LastCalculatedAt = DateTime.UtcNow.AddHours(-3)
                }
            };

            // Calculate portfolio values and add holdings
            foreach (var portfolio in portfolios)
            {
                CalculatePortfolioValues(portfolio);
            }

            return portfolios;
        }

        private void CalculatePortfolioValues(PortfolioDto portfolio)
        {
            var portfolioTransactions = _transactions.Where(t => t.PortfolioId == portfolio.Id).ToList();
            var holdings = new Dictionary<string, HoldingDto>();

            // Calculate holdings from transactions
            foreach (var transaction in portfolioTransactions)
            {
                if (!holdings.ContainsKey(transaction.Symbol))
                {
                    holdings[transaction.Symbol] = new HoldingDto
                    {
                        Id = holdings.Count + 1,
                        PortfolioId = portfolio.Id,
                        Symbol = transaction.Symbol,
                        CompanyName = transaction.CompanyName,
                        Quantity = 0,
                        TotalCost = 0,
                        LastUpdated = DateTime.UtcNow,
                        CreatedAt = transaction.CreatedAt
                    };
                }

                var holding = holdings[transaction.Symbol];
                if (transaction.Type == "BUY")
                {
                    var newTotalCost = holding.TotalCost + transaction.TotalAmount + transaction.Fees;
                    var newQuantity = holding.Quantity + transaction.Quantity;
                    holding.AverageCost = newQuantity > 0 ? newTotalCost / newQuantity : 0;
                    holding.TotalCost = newTotalCost;
                    holding.Quantity = newQuantity;
                }
                else if (transaction.Type == "SELL")
                {
                    var soldCost = holding.AverageCost * transaction.Quantity;
                    holding.TotalCost -= soldCost;
                    holding.Quantity -= transaction.Quantity;
                    if (holding.Quantity > 0)
                    {
                        holding.AverageCost = holding.TotalCost / holding.Quantity;
                    }
                }
            }

            // Calculate current values
            double totalCurrentValue = 0;
            double totalCost = 0;

            foreach (var holding in holdings.Values.Where(h => h.Quantity > 0))
            {
                var currentPrice = _stockPrices.GetValueOrDefault(holding.Symbol, holding.AverageCost);
                holding.CurrentPrice = currentPrice;
                holding.CurrentValue = holding.Quantity * currentPrice;
                holding.UnrealizedGainLoss = holding.CurrentValue - holding.TotalCost;
                holding.UnrealizedGainLossPercent = holding.TotalCost > 0 ? (holding.UnrealizedGainLoss / holding.TotalCost) * 100 : 0;

                // Mock day changes
                var random = new Random(holding.Symbol.GetHashCode());
                holding.DayChange = (random.NextDouble() - 0.5) * 10; // -5 to +5
                holding.DayChangePercent = holding.CurrentPrice > 0 ? (holding.DayChange / holding.CurrentPrice) * 100 : 0;

                totalCurrentValue += holding.CurrentValue;
                totalCost += holding.TotalCost;
            }

            // Calculate portfolio percentages
            foreach (var holding in holdings.Values.Where(h => h.Quantity > 0))
            {
                holding.PortfolioPercent = totalCurrentValue > 0 ? (holding.CurrentValue / totalCurrentValue) * 100 : 0;
            }

            portfolio.Holdings = holdings.Values.Where(h => h.Quantity > 0).ToList();
            portfolio.CurrentValue = totalCurrentValue;
            portfolio.TotalGainLoss = totalCurrentValue - totalCost;
            portfolio.TotalGainLossPercent = totalCost > 0 ? (portfolio.TotalGainLoss / totalCost) * 100 : 0;
            portfolio.TotalHoldings = portfolio.Holdings.Count;
            portfolio.TotalTransactions = portfolioTransactions.Count;
        }

        public async Task<PaginatedResult<PortfolioSummaryDto>> GetPortfoliosAsync(int page = 1, int pageSize = 10)
        {
            await Task.Delay(200); // Simulate API delay

            var userPortfolios = _portfolios
                .Where(p => p.UserId == _currentUserId)
                .OrderByDescending(p => p.UpdatedAt)
                .ToList();

            var totalCount = userPortfolios.Count;
            var portfolios = userPortfolios
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
                    TotalHoldings = p.TotalHoldings,
                    LastCalculatedAt = p.LastCalculatedAt ?? p.UpdatedAt,
                    CreatedAt = p.CreatedAt
                })
                .ToList();

            return new PaginatedResult<PortfolioSummaryDto>
            {
                Data = portfolios,
                TotalRecords = totalCount,
                PageNumber = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

        public async Task<PortfolioDto?> GetPortfolioAsync(int id)
        {
            await Task.Delay(150); // Simulate API delay

            var portfolio = _portfolios.FirstOrDefault(p => p.Id == id && p.UserId == _currentUserId);
            if (portfolio != null)
            {
                // Recalculate to ensure fresh data
                CalculatePortfolioValues(portfolio);
            }
            return portfolio;
        }

        public async Task<PortfolioDto?> CreatePortfolioAsync(CreatePortfolioDto createDto)
        {
            await Task.Delay(300); // Simulate API delay

            var portfolio = new PortfolioDto
            {
                Id = _nextPortfolioId++,
                UserId = _currentUserId,
                Name = createDto.Name,
                Description = createDto.Description,
                Type = createDto.Type,
                InitialValue = createDto.InitialValue,
                CurrentValue = createDto.InitialValue,
                TotalGainLoss = 0,
                TotalGainLossPercent = 0,
                IsActive = true,
                IsPublic = createDto.IsPublic,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                LastCalculatedAt = DateTime.UtcNow,
                TotalHoldings = 0,
                TotalTransactions = 0,
                Holdings = new List<HoldingDto>()
            };

            _portfolios.Add(portfolio);
            return portfolio;
        }

        public async Task<PortfolioDto?> UpdatePortfolioAsync(int id, UpdatePortfolioDto updateDto)
        {
            await Task.Delay(250); // Simulate API delay

            var portfolio = _portfolios.FirstOrDefault(p => p.Id == id && p.UserId == _currentUserId);
            if (portfolio != null)
            {
                portfolio.Name = updateDto.Name;
                portfolio.Description = updateDto.Description;
                portfolio.Type = updateDto.Type;
                portfolio.IsPublic = updateDto.IsPublic;
                portfolio.UpdatedAt = DateTime.UtcNow;
                return portfolio;
            }

            return null;
        }

        public async Task<bool> DeletePortfolioAsync(int id)
        {
            await Task.Delay(200); // Simulate API delay

            var portfolio = _portfolios.FirstOrDefault(p => p.Id == id && p.UserId == _currentUserId);
            if (portfolio != null)
            {
                _portfolios.Remove(portfolio);
                // Also remove related transactions
                _transactions.RemoveAll(t => t.PortfolioId == id);
                return true;
            }

            return false;
        }

        public async Task<bool> RecalculatePortfolioAsync(int id)
        {
            await Task.Delay(500); // Simulate API delay

            var portfolio = _portfolios.FirstOrDefault(p => p.Id == id && p.UserId == _currentUserId);
            if (portfolio != null)
            {
                CalculatePortfolioValues(portfolio);
                portfolio.LastCalculatedAt = DateTime.UtcNow;
                portfolio.UpdatedAt = DateTime.UtcNow;
                return true;
            }

            return false;
        }

        public async Task<List<PortfolioSummaryDto>> GetPublicPortfoliosAsync(int page = 1, int pageSize = 20)
        {
            await Task.Delay(200); // Simulate API delay

            return _portfolios
                .Where(p => p.IsPublic)
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
                    TotalHoldings = p.TotalHoldings,
                    LastCalculatedAt = p.LastCalculatedAt ?? p.UpdatedAt,
                    CreatedAt = p.CreatedAt
                })
                .ToList();
        }

        public async Task<PaginatedResult<TransactionDto>> GetPortfolioTransactionsAsync(int portfolioId, int page = 1, int pageSize = 20)
        {
            await Task.Delay(150); // Simulate API delay

            var portfolioTransactions = _transactions
                .Where(t => t.PortfolioId == portfolioId)
                .OrderByDescending(t => t.TransactionDate)
                .ToList();

            var totalCount = portfolioTransactions.Count;
            var transactions = portfolioTransactions
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedResult<TransactionDto>
            {
                Data = transactions,
                TotalRecords = totalCount,
                PageNumber = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

        public async Task<TransactionDto?> GetTransactionAsync(int id)
        {
            await Task.Delay(100); // Simulate API delay

            return _transactions.FirstOrDefault(t => t.Id == id);
        }

        public async Task<TransactionDto?> CreateTransactionAsync(CreateTransactionDto createDto)
        {
            await Task.Delay(300); // Simulate API delay

            var transactionDate = createDto.TransactionDate ?? DateTime.UtcNow;
            var totalAmount = createDto.Quantity * createDto.Price;
            var netAmount = createDto.Type == "BUY" ? totalAmount + createDto.Fees : totalAmount - createDto.Fees;

            var transaction = new TransactionDto
            {
                Id = _nextTransactionId++,
                PortfolioId = createDto.PortfolioId,
                Symbol = createDto.Symbol.ToUpperInvariant(),
                CompanyName = _companyNames.GetValueOrDefault(createDto.Symbol.ToUpperInvariant(), createDto.Symbol.ToUpperInvariant()),
                Type = createDto.Type.ToUpperInvariant(),
                Quantity = createDto.Quantity,
                Price = createDto.Price,
                TotalAmount = totalAmount,
                Fees = createDto.Fees,
                NetAmount = netAmount,
                Notes = createDto.Notes,
                TransactionDate = transactionDate,
                CreatedAt = DateTime.UtcNow
            };

            _transactions.Add(transaction);

            // Recalculate portfolio
            var portfolio = _portfolios.FirstOrDefault(p => p.Id == createDto.PortfolioId);
            if (portfolio != null)
            {
                CalculatePortfolioValues(portfolio);
                portfolio.UpdatedAt = DateTime.UtcNow;
            }

            return transaction;
        }

        public async Task<bool> DeleteTransactionAsync(int id)
        {
            await Task.Delay(200); // Simulate API delay

            var transaction = _transactions.FirstOrDefault(t => t.Id == id);
            if (transaction != null)
            {
                _transactions.Remove(transaction);

                // Recalculate portfolio
                var portfolio = _portfolios.FirstOrDefault(p => p.Id == transaction.PortfolioId);
                if (portfolio != null)
                {
                    CalculatePortfolioValues(portfolio);
                    portfolio.UpdatedAt = DateTime.UtcNow;
                }

                return true;
            }

            return false;
        }

        public async Task<List<TransactionDto>> GetSymbolTransactionsAsync(int portfolioId, string symbol)
        {
            await Task.Delay(100); // Simulate API delay

            return _transactions
                .Where(t => t.PortfolioId == portfolioId && t.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(t => t.TransactionDate)
                .ToList();
        }

        public async Task<List<TransactionDto>> GetRecentTransactionsAsync(int count = 10)
        {
            await Task.Delay(100); // Simulate API delay

            var userPortfolioIds = _portfolios
                .Where(p => p.UserId == _currentUserId)
                .Select(p => p.Id)
                .ToList();

            return _transactions
                .Where(t => userPortfolioIds.Contains(t.PortfolioId))
                .OrderByDescending(t => t.TransactionDate)
                .Take(count)
                .ToList();
        }

        // Helper method to set current user (useful for testing different users)
        public void SetCurrentUser(string userId)
        {
            _currentUserId = userId;
        }

        // Helper method to update stock prices (useful for testing price changes)
        public void UpdateStockPrice(string symbol, double price)
        {
            _stockPrices[symbol.ToUpperInvariant()] = price;

            // Recalculate all portfolios that hold this stock
            foreach (var portfolio in _portfolios)
            {
                if (portfolio.Holdings.Any(h => h.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase)))
                {
                    CalculatePortfolioValues(portfolio);
                    portfolio.UpdatedAt = DateTime.UtcNow;
                    portfolio.LastCalculatedAt = DateTime.UtcNow;
                }
            }
        }
    }
}