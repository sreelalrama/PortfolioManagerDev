using StockTradePro.BlazorUI.Models.Watchlist;
using StockTradePro.BlazorUI.Services;

namespace StockTradePro.BlazorUI.Services.Mocks
{
    public class MockWatchlistService : IWatchlistService
    {
        private readonly List<WatchlistDto> _watchlists;
        private readonly List<WatchlistItemDto> _watchlistItems;
        private readonly List<PriceAlertDto> _priceAlerts;
        private readonly Dictionary<string, StockPriceDto> _stockPrices;
        private int _nextWatchlistId;
        private int _nextItemId;
        private int _nextAlertId;
        private string _currentUserId = "1"; // Default to user "1" for testing

        public MockWatchlistService()
        {
            _nextWatchlistId = 1;
            _nextItemId = 1;
            _nextAlertId = 1;
            _stockPrices = GenerateStockPrices();
            _watchlists = GenerateMockWatchlists();
            _watchlistItems = GenerateMockWatchlistItems();
            _priceAlerts = GenerateMockPriceAlerts();
        }

        private Dictionary<string, StockPriceDto> GenerateStockPrices()
        {
            return new Dictionary<string, StockPriceDto>
            {
                { "AAPL", new StockPriceDto { Symbol = "AAPL", Price = 175.25, Change = 2.45, ChangePercent = 1.42, LastUpdated = DateTime.UtcNow.AddMinutes(-5) } },
                { "GOOGL", new StockPriceDto { Symbol = "GOOGL", Price = 142.50, Change = -1.25, ChangePercent = -0.87, LastUpdated = DateTime.UtcNow.AddMinutes(-3) } },
                { "MSFT", new StockPriceDto { Symbol = "MSFT", Price = 378.90, Change = 5.67, ChangePercent = 1.52, LastUpdated = DateTime.UtcNow.AddMinutes(-7) } },
                { "AMZN", new StockPriceDto { Symbol = "AMZN", Price = 145.75, Change = -2.33, ChangePercent = -1.58, LastUpdated = DateTime.UtcNow.AddMinutes(-2) } },
                { "TSLA", new StockPriceDto { Symbol = "TSLA", Price = 248.50, Change = 12.45, ChangePercent = 5.27, LastUpdated = DateTime.UtcNow.AddMinutes(-1) } },
                { "META", new StockPriceDto { Symbol = "META", Price = 325.80, Change = -8.90, ChangePercent = -2.66, LastUpdated = DateTime.UtcNow.AddMinutes(-4) } },
                { "NVDA", new StockPriceDto { Symbol = "NVDA", Price = 875.25, Change = 23.45, ChangePercent = 2.76, LastUpdated = DateTime.UtcNow.AddMinutes(-6) } },
                { "NFLX", new StockPriceDto { Symbol = "NFLX", Price = 445.60, Change = -5.40, ChangePercent = -1.20, LastUpdated = DateTime.UtcNow.AddMinutes(-8) } },
                { "AMD", new StockPriceDto { Symbol = "AMD", Price = 118.75, Change = 3.25, ChangePercent = 2.81, LastUpdated = DateTime.UtcNow.AddMinutes(-9) } },
                { "INTC", new StockPriceDto { Symbol = "INTC", Price = 35.20, Change = 0.85, ChangePercent = 2.47, LastUpdated = DateTime.UtcNow.AddMinutes(-10) } },
                { "JPM", new StockPriceDto { Symbol = "JPM", Price = 147.85, Change = 2.15, ChangePercent = 1.48, LastUpdated = DateTime.UtcNow.AddMinutes(-11) } },
                { "V", new StockPriceDto { Symbol = "V", Price = 226.75, Change = 3.20, ChangePercent = 1.43, LastUpdated = DateTime.UtcNow.AddMinutes(-12) } },
                { "JNJ", new StockPriceDto { Symbol = "JNJ", Price = 162.35, Change = 1.45, ChangePercent = 0.90, LastUpdated = DateTime.UtcNow.AddMinutes(-13) } },
                { "KO", new StockPriceDto { Symbol = "KO", Price = 61.75, Change = 0.45, ChangePercent = 0.73, LastUpdated = DateTime.UtcNow.AddMinutes(-14) } },
                { "DIS", new StockPriceDto { Symbol = "DIS", Price = 101.25, Change = -1.45, ChangePercent = -1.41, LastUpdated = DateTime.UtcNow.AddMinutes(-15) } }
            };
        }

        private List<WatchlistDto> GenerateMockWatchlists()
        {
            return new List<WatchlistDto>
            {
                new WatchlistDto
                {
                    Id = _nextWatchlistId++,
                    Name = "My Watchlist",
                    Description = "Default watchlist for tracking my favorite stocks",
                    UserId = "1",
                    IsDefault = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    UpdatedAt = DateTime.UtcNow.AddHours(-2),
                    Items = new List<WatchlistItemDto>(),
                    PriceAlerts = new List<PriceAlertDto>()
                },
                new WatchlistDto
                {
                    Id = _nextWatchlistId++,
                    Name = "Tech Giants",
                    Description = "Major technology companies to watch",
                    UserId = "1",
                    IsDefault = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-20),
                    UpdatedAt = DateTime.UtcNow.AddDays(-1),
                    Items = new List<WatchlistItemDto>(),
                    PriceAlerts = new List<PriceAlertDto>()
                },
                new WatchlistDto
                {
                    Id = _nextWatchlistId++,
                    Name = "Growth Stocks",
                    Description = "High growth potential stocks",
                    UserId = "1",
                    IsDefault = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-15),
                    UpdatedAt = DateTime.UtcNow.AddDays(-3),
                    Items = new List<WatchlistItemDto>(),
                    PriceAlerts = new List<PriceAlertDto>()
                },
                new WatchlistDto
                {
                    Id = _nextWatchlistId++,
                    Name = "Blue Chips",
                    Description = "Stable, dividend-paying stocks",
                    UserId = "2",
                    IsDefault = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-25),
                    UpdatedAt = DateTime.UtcNow.AddDays(-5),
                    Items = new List<WatchlistItemDto>(),
                    PriceAlerts = new List<PriceAlertDto>()
                }
            };
        }

        private List<WatchlistItemDto> GenerateMockWatchlistItems()
        {
            var items = new List<WatchlistItemDto>();

            // Default watchlist items
            var defaultWatchlistItems = new[]
            {
                ("AAPL", 1), ("GOOGL", 2), ("MSFT", 3), ("AMZN", 4), ("TSLA", 5)
            };

            foreach (var (symbol, order) in defaultWatchlistItems)
            {
                items.Add(CreateWatchlistItem(1, symbol, order, DateTime.UtcNow.AddDays(-30 + order)));
            }

            // Tech Giants watchlist items
            var techGiantItems = new[]
            {
                ("AAPL", 1), ("GOOGL", 2), ("MSFT", 3), ("META", 4), ("NVDA", 5), ("NFLX", 6)
            };

            foreach (var (symbol, order) in techGiantItems)
            {
                items.Add(CreateWatchlistItem(2, symbol, order, DateTime.UtcNow.AddDays(-20 + order)));
            }

            // Growth Stocks watchlist items
            var growthStockItems = new[]
            {
                ("TSLA", 1), ("NVDA", 2), ("AMD", 3), ("NFLX", 4)
            };

            foreach (var (symbol, order) in growthStockItems)
            {
                items.Add(CreateWatchlistItem(3, symbol, order, DateTime.UtcNow.AddDays(-15 + order)));
            }

            // Blue Chips watchlist items (for user 2)
            var blueChipItems = new[]
            {
                ("JPM", 1), ("JNJ", 2), ("KO", 3), ("V", 4), ("DIS", 5)
            };

            foreach (var (symbol, order) in blueChipItems)
            {
                items.Add(CreateWatchlistItem(4, symbol, order, DateTime.UtcNow.AddDays(-25 + order)));
            }

            return items;
        }

        private WatchlistItemDto CreateWatchlistItem(int watchlistId, string symbol, int sortOrder, DateTime addedAt)
        {
            return new WatchlistItemDto
            {
                Id = _nextItemId++,
                WatchlistId = watchlistId,
                Symbol = symbol,
                SortOrder = sortOrder,
                AddedAt = addedAt,
                CurrentPrice = _stockPrices.GetValueOrDefault(symbol)
            };
        }

        private List<PriceAlertDto> GenerateMockPriceAlerts()
        {
            return new List<PriceAlertDto>
            {
                new PriceAlertDto
                {
                    Id = _nextAlertId++,
                    WatchlistId = 1,
                    Symbol = "AAPL",
                    Type = AlertType.Above,
                    TargetValue = 180.00,
                    Status = AlertStatus.Active,
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    Notes = "Alert when AAPL crosses $180"
                },
                new PriceAlertDto
                {
                    Id = _nextAlertId++,
                    WatchlistId = 1,
                    Symbol = "TSLA",
                    Type = AlertType.Below,
                    TargetValue = 240.00,
                    Status = AlertStatus.Triggered,
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    TriggeredAt = DateTime.UtcNow.AddHours(-2),
                    Notes = "Stop loss alert for TSLA"
                },
                new PriceAlertDto
                {
                    Id = _nextAlertId++,
                    WatchlistId = 2,
                    Symbol = "GOOGL",
                    Type = AlertType.PercentUp,
                    TargetValue = 5.0,
                    Status = AlertStatus.Active,
                    CreatedAt = DateTime.UtcNow.AddDays(-7),
                    Notes = "Alert when GOOGL gains 5% in a day"
                },
                new PriceAlertDto
                {
                    Id = _nextAlertId++,
                    WatchlistId = 2,
                    Symbol = "NVDA",
                    Type = AlertType.Above,
                    TargetValue = 900.00,
                    Status = AlertStatus.Active,
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    Notes = "Buy signal when NVDA hits $900"
                },
                new PriceAlertDto
                {
                    Id = _nextAlertId++,
                    WatchlistId = 3,
                    Symbol = "AMD",
                    Type = AlertType.PercentDown,
                    TargetValue = 10.0,
                    Status = AlertStatus.Disabled,
                    CreatedAt = DateTime.UtcNow.AddDays(-12),
                    Notes = "Buying opportunity alert for AMD"
                }
            };
        }

        public async Task<List<WatchlistDto>> GetWatchlistsAsync()
        {
            await Task.Delay(200); // Simulate API delay

            var userWatchlists = _watchlists
                .Where(w => w.UserId == _currentUserId)
                .OrderBy(w => w.IsDefault ? 0 : 1)
                .ThenBy(w => w.Name)
                .ToList();

            // Populate items and alerts for each watchlist
            foreach (var watchlist in userWatchlists)
            {
                watchlist.Items = _watchlistItems
                    .Where(i => i.WatchlistId == watchlist.Id)
                    .OrderBy(i => i.SortOrder)
                    .ToList();

                watchlist.PriceAlerts = _priceAlerts
                    .Where(a => a.WatchlistId == watchlist.Id)
                    .OrderByDescending(a => a.CreatedAt)
                    .ToList();
            }

            return userWatchlists;
        }

        public async Task<WatchlistDto?> GetWatchlistAsync(int id)
        {
            await Task.Delay(150); // Simulate API delay

            var watchlist = _watchlists
                .FirstOrDefault(w => w.Id == id && w.UserId == _currentUserId);

            if (watchlist != null)
            {
                watchlist.Items = _watchlistItems
                    .Where(i => i.WatchlistId == watchlist.Id)
                    .OrderBy(i => i.SortOrder)
                    .ToList();

                watchlist.PriceAlerts = _priceAlerts
                    .Where(a => a.WatchlistId == watchlist.Id)
                    .OrderByDescending(a => a.CreatedAt)
                    .ToList();

                // Update current prices
                foreach (var item in watchlist.Items)
                {
                    item.CurrentPrice = _stockPrices.GetValueOrDefault(item.Symbol);
                }
            }

            return watchlist;
        }

        public async Task<WatchlistDto?> CreateWatchlistAsync(CreateWatchlistDto createDto)
        {
            await Task.Delay(300); // Simulate API delay

            // If this is set as default, make all other watchlists non-default
            if (createDto.IsDefault)
            {
                foreach (var existingWatchlist in _watchlists.Where(w => w.UserId == _currentUserId))
                {
                    existingWatchlist.IsDefault = false;
                }
            }

            var watchlist = new WatchlistDto
            {
                Id = _nextWatchlistId++,
                Name = createDto.Name,
                Description = createDto.Description,
                UserId = _currentUserId,
                IsDefault = createDto.IsDefault,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Items = new List<WatchlistItemDto>(),
                PriceAlerts = new List<PriceAlertDto>()
            };

            _watchlists.Add(watchlist);
            return watchlist;
        }

        public async Task<WatchlistDto?> UpdateWatchlistAsync(int id, UpdateWatchlistDto updateDto)
        {
            await Task.Delay(250); // Simulate API delay

            var watchlist = _watchlists
                .FirstOrDefault(w => w.Id == id && w.UserId == _currentUserId);

            if (watchlist != null)
            {
                watchlist.Name = updateDto.Name;
                watchlist.Description = updateDto.Description;
                watchlist.UpdatedAt = DateTime.UtcNow;
                return watchlist;
            }

            return null;
        }

        public async Task<bool> DeleteWatchlistAsync(int id)
        {
            await Task.Delay(200); // Simulate API delay

            var watchlist = _watchlists
                .FirstOrDefault(w => w.Id == id && w.UserId == _currentUserId);

            if (watchlist != null)
            {
                _watchlists.Remove(watchlist);

                // Remove all items and alerts for this watchlist
                _watchlistItems.RemoveAll(i => i.WatchlistId == id);
                _priceAlerts.RemoveAll(a => a.WatchlistId == id);

                return true;
            }

            return false;
        }

        public async Task<List<WatchlistItemDto>> GetWatchlistItemsAsync(int watchlistId)
        {
            await Task.Delay(150); // Simulate API delay

            var items = _watchlistItems
                .Where(i => i.WatchlistId == watchlistId)
                .OrderBy(i => i.SortOrder)
                .ToList();

            // Update current prices
            foreach (var item in items)
            {
                item.CurrentPrice = _stockPrices.GetValueOrDefault(item.Symbol);
            }

            return items;
        }

        public async Task<WatchlistItemDto?> AddWatchlistItemAsync(int watchlistId, AddWatchlistItemDto addDto)
        {
            await Task.Delay(250); // Simulate API delay

            // Check if watchlist exists and belongs to current user
            var watchlist = _watchlists
                .FirstOrDefault(w => w.Id == watchlistId && w.UserId == _currentUserId);

            if (watchlist == null)
                return null;

            // Check if symbol already exists in this watchlist
            if (_watchlistItems.Any(i => i.WatchlistId == watchlistId &&
                                        i.Symbol.Equals(addDto.Symbol, StringComparison.OrdinalIgnoreCase)))
            {
                return null; // Symbol already exists
            }

            var item = new WatchlistItemDto
            {
                Id = _nextItemId++,
                WatchlistId = watchlistId,
                Symbol = addDto.Symbol.ToUpperInvariant(),
                SortOrder = addDto.SortOrder,
                AddedAt = DateTime.UtcNow,
                CurrentPrice = _stockPrices.GetValueOrDefault(addDto.Symbol.ToUpperInvariant())
            };

            _watchlistItems.Add(item);

            // Update watchlist timestamp
            watchlist.UpdatedAt = DateTime.UtcNow;

            return item;
        }

        public async Task<bool> DeleteWatchlistItemAsync(int watchlistId, int itemId)
        {
            await Task.Delay(200); // Simulate API delay

            var item = _watchlistItems
                .FirstOrDefault(i => i.Id == itemId && i.WatchlistId == watchlistId);

            if (item != null)
            {
                _watchlistItems.Remove(item);

                // Update watchlist timestamp
                var watchlist = _watchlists.FirstOrDefault(w => w.Id == watchlistId);
                if (watchlist != null)
                {
                    watchlist.UpdatedAt = DateTime.UtcNow;
                }

                return true;
            }

            return false;
        }

        public async Task<bool> UpdateItemSortOrderAsync(int watchlistId, int itemId, int sortOrder)
        {
            await Task.Delay(150); // Simulate API delay

            var item = _watchlistItems
                .FirstOrDefault(i => i.Id == itemId && i.WatchlistId == watchlistId);

            if (item != null)
            {
                item.SortOrder = sortOrder;

                // Update watchlist timestamp
                var watchlist = _watchlists.FirstOrDefault(w => w.Id == watchlistId);
                if (watchlist != null)
                {
                    watchlist.UpdatedAt = DateTime.UtcNow;
                }

                return true;
            }

            return false;
        }

        public async Task<List<PriceAlertDto>> GetPriceAlertsAsync(int watchlistId)
        {
            await Task.Delay(150); // Simulate API delay

            return _priceAlerts
                .Where(a => a.WatchlistId == watchlistId)
                .OrderByDescending(a => a.CreatedAt)
                .ToList();
        }

        public async Task<PriceAlertDto?> GetPriceAlertAsync(int watchlistId, int alertId)
        {
            await Task.Delay(100); // Simulate API delay

            return _priceAlerts
                .FirstOrDefault(a => a.Id == alertId && a.WatchlistId == watchlistId);
        }

        public async Task<PriceAlertDto?> CreatePriceAlertAsync(int watchlistId, CreatePriceAlertDto createDto)
        {
            await Task.Delay(250); // Simulate API delay

            // Check if watchlist exists and belongs to current user
            var watchlist = _watchlists
                .FirstOrDefault(w => w.Id == watchlistId && w.UserId == _currentUserId);

            if (watchlist == null)
                return null;

            var alert = new PriceAlertDto
            {
                Id = _nextAlertId++,
                WatchlistId = watchlistId,
                Symbol = createDto.Symbol.ToUpperInvariant(),
                Type = createDto.Type,
                TargetValue = createDto.TargetValue,
                Status = AlertStatus.Active,
                CreatedAt = DateTime.UtcNow,
                Notes = createDto.Notes ?? string.Empty
            };

            _priceAlerts.Add(alert);
            return alert;
        }

        public async Task<bool> DeletePriceAlertAsync(int watchlistId, int alertId)
        {
            await Task.Delay(200); // Simulate API delay

            var alert = _priceAlerts
                .FirstOrDefault(a => a.Id == alertId && a.WatchlistId == watchlistId);

            if (alert != null)
            {
                _priceAlerts.Remove(alert);
                return true;
            }

            return false;
        }

        public async Task<bool> DisablePriceAlertAsync(int watchlistId, int alertId)
        {
            await Task.Delay(150); // Simulate API delay

            var alert = _priceAlerts
                .FirstOrDefault(a => a.Id == alertId && a.WatchlistId == watchlistId);

            if (alert != null)
            {
                alert.Status = AlertStatus.Disabled;
                return true;
            }

            return false;
        }

        public async Task<List<PriceAlertDto>> GetActiveAlertsAsync()
        {
            await Task.Delay(150); // Simulate API delay

            var userWatchlistIds = _watchlists
                .Where(w => w.UserId == _currentUserId)
                .Select(w => w.Id)
                .ToList();

            return _priceAlerts
                .Where(a => userWatchlistIds.Contains(a.WatchlistId) && a.Status == AlertStatus.Active)
                .OrderByDescending(a => a.CreatedAt)
                .ToList();
        }

        // Helper methods for testing
        public void SetCurrentUser(string userId)
        {
            _currentUserId = userId;
        }

        public void UpdateStockPrice(string symbol, double price, double change, double changePercent)
        {
            var stockPrice = _stockPrices.GetValueOrDefault(symbol.ToUpperInvariant());
            if (stockPrice != null)
            {
                stockPrice.Price = price;
                stockPrice.Change = change;
                stockPrice.ChangePercent = changePercent;
                stockPrice.LastUpdated = DateTime.UtcNow;
            }
            else
            {
                _stockPrices[symbol.ToUpperInvariant()] = new StockPriceDto
                {
                    Symbol = symbol.ToUpperInvariant(),
                    Price = price,
                    Change = change,
                    ChangePercent = changePercent,
                    LastUpdated = DateTime.UtcNow
                };
            }

            // Update watchlist items
            var items = _watchlistItems.Where(i => i.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase));
            foreach (var item in items)
            {
                item.CurrentPrice = _stockPrices[symbol.ToUpperInvariant()];
            }

            // Check and trigger alerts
            CheckAndTriggerAlerts(symbol.ToUpperInvariant(), price, changePercent);
        }

        private void CheckAndTriggerAlerts(string symbol, double currentPrice, double changePercent)
        {
            var activeAlerts = _priceAlerts
                .Where(a => a.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase) &&
                           a.Status == AlertStatus.Active)
                .ToList();

            foreach (var alert in activeAlerts)
            {
                bool shouldTrigger = alert.Type switch
                {
                    AlertType.Above => currentPrice >= alert.TargetValue,
                    AlertType.Below => currentPrice <= alert.TargetValue,
                    AlertType.PercentUp => changePercent >= alert.TargetValue,
                    AlertType.PercentDown => changePercent <= -alert.TargetValue,
                    _ => false
                };

                if (shouldTrigger)
                {
                    alert.Status = AlertStatus.Triggered;
                    alert.TriggeredAt = DateTime.UtcNow;
                }
            }
        }

        public void SimulateMarketMovement()
        {
            var random = new Random();

            foreach (var stockPrice in _stockPrices.Values)
            {
                var changePercent = (random.NextDouble() - 0.5) * 10; // ±5%
                var change = stockPrice.Price * (changePercent / 100);
                var newPrice = stockPrice.Price + change;

                UpdateStockPrice(stockPrice.Symbol, newPrice, change, changePercent);
            }
        }
    }
}