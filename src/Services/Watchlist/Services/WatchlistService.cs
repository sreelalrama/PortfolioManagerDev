

using Microsoft.EntityFrameworkCore;
using StockTradePro.WatchlistService.API.Data;
using StockTradePro.WatchlistService.API.DTOs;
using StockTradePro.WatchlistService.API.Models;

namespace StockTradePro.WatchlistService.API.Services
{
    public class WatchlistServiceImpl : IWatchlistService
    {
        private readonly WatchlistDbContext _context;
        private readonly IStockDataService _stockDataService;
        private readonly ILogger<WatchlistServiceImpl> _logger;

        public WatchlistServiceImpl(
            WatchlistDbContext context,
            IStockDataService stockDataService,
            ILogger<WatchlistServiceImpl> logger)
        {
            _context = context;
            _stockDataService = stockDataService;
            _logger = logger;
        }

        public async Task<IEnumerable<WatchlistDto>> GetUserWatchlistsAsync(string userId)
        {
            var watchlists = await _context.Watchlists
                .Where(w => w.UserId == userId)
                .Include(w => w.Items)
                .Include(w => w.PriceAlerts)
                .OrderBy(w => w.Name)
                .ToListAsync();

            return watchlists.Select(MapToDto);
        }

        public async Task<WatchlistDto?> GetWatchlistAsync(int id, string userId)
        {
            var watchlist = await _context.Watchlists
                .Where(w => w.Id == id && w.UserId == userId)
                .Include(w => w.Items)
                .Include(w => w.PriceAlerts)
                .FirstOrDefaultAsync();

            return watchlist != null ? MapToDto(watchlist) : null;
        }

        public async Task<WatchlistDto> CreateWatchlistAsync(CreateWatchlistDto dto, string userId)
        {
            // Check if user already has a default watchlist
            if (dto.IsDefault)
            {
                await _context.Watchlists
                    .Where(w => w.UserId == userId && w.IsDefault)
                    .ExecuteUpdateAsync(w => w.SetProperty(p => p.IsDefault, false));
            }
            var watchlist = new Watchlist
            {
                Name = dto.Name,
                Description = dto.Description,
                UserId = userId,
                IsDefault = dto.IsDefault,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Watchlists.Add(watchlist);
            await _context.SaveChangesAsync();

            return MapToDto(watchlist);
        }

        public async Task<WatchlistDto?> UpdateWatchlistAsync(int id, UpdateWatchlistDto dto, string userId)
        {
            var watchlist = await _context.Watchlists
                .Where(w => w.Id == id && w.UserId == userId)
                .FirstOrDefaultAsync();

            if (watchlist == null) return null;

            watchlist.Name = dto.Name;
            watchlist.Description = dto.Description;
            watchlist.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return MapToDto(watchlist);
        }

        public async Task<bool> DeleteWatchlistAsync(int id, string userId)
        {
            var watchlist = await _context.Watchlists
                .Where(w => w.Id == id && w.UserId == userId)
                .FirstOrDefaultAsync();

            if (watchlist == null) return false;

            _context.Watchlists.Remove(watchlist);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<WatchlistItemDto?> AddItemToWatchlistAsync(int watchlistId, AddWatchlistItemDto dto, string userId)
        {
            var watchlist = await _context.Watchlists
                .Where(w => w.Id == watchlistId && w.UserId == userId)
                .FirstOrDefaultAsync();

            if (watchlist == null) return null;

            // Verify stock exists
            var stockExists = await _stockDataService.GetStockAsync(dto.Symbol);
            if (stockExists == null)
            {
                _logger.LogWarning("Attempted to add non-existent stock {Symbol} to watchlist {WatchlistId}", dto.Symbol, watchlistId);
                return null;
            }

            // Check if item already exists
            var existingItem = await _context.WatchlistItems
                .Where(i => i.WatchlistId == watchlistId && i.Symbol == dto.Symbol)
                .FirstOrDefaultAsync();

            if (existingItem != null) return MapToItemDto(existingItem);

            var item = new WatchlistItem
            {
                WatchlistId = watchlistId,
                Symbol = dto.Symbol.ToUpper(),
                SortOrder = dto.SortOrder,
                AddedAt = DateTime.UtcNow
            };

            _context.WatchlistItems.Add(item);
            await _context.SaveChangesAsync();

            return MapToItemDto(item);
        }

        public async Task<bool> RemoveItemFromWatchlistAsync(int watchlistId, int itemId, string userId)
        {
            var item = await _context.WatchlistItems
                .Where(i => i.Id == itemId && i.WatchlistId == watchlistId)
                .Include(i => i.Watchlist)
                .FirstOrDefaultAsync();

            if (item?.Watchlist?.UserId != userId) return false;

            _context.WatchlistItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateItemSortOrderAsync(int watchlistId, int itemId, int newSortOrder, string userId)
        {
            var item = await _context.WatchlistItems
                .Where(i => i.Id == itemId && i.WatchlistId == watchlistId)
                .Include(i => i.Watchlist)
                .FirstOrDefaultAsync();

            if (item?.Watchlist?.UserId != userId) return false;

            item.SortOrder = newSortOrder;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<WatchlistItemDto>> GetWatchlistItemsWithPricesAsync(int watchlistId, string userId)
        {
            var items = await _context.WatchlistItems
                .Where(i => i.WatchlistId == watchlistId)
                .Include(i => i.Watchlist)
                .Where(i => i.Watchlist.UserId == userId)
                .OrderBy(i => i.SortOrder)
                .ToListAsync();

            var itemDtos = new List<WatchlistItemDto>();

            foreach (var item in items)
            {
                var dto = MapToItemDto(item);
                dto.CurrentPrice = await _stockDataService.GetCurrentPriceAsync(item.Symbol);
                itemDtos.Add(dto);
            }

            return itemDtos;
        }
        private WatchlistDto MapToDto(Watchlist watchlist)
        {
            return new WatchlistDto
            {
                Id = watchlist.Id,
                Name = watchlist.Name,
                Description = watchlist.Description,
                UserId = watchlist.UserId,
                IsDefault = watchlist.IsDefault,
                CreatedAt = watchlist.CreatedAt,
                UpdatedAt = watchlist.UpdatedAt,
                Items = watchlist.Items?.Select(MapToItemDto).ToList() ?? new(),
                PriceAlerts = watchlist.PriceAlerts?.Select(MapToAlertDto).ToList() ?? new()
            };
        }

        private WatchlistItemDto MapToItemDto(WatchlistItem item)
        {
            return new WatchlistItemDto
            {
                Id = item.Id,
                WatchlistId = item.WatchlistId,
                Symbol = item.Symbol,
                SortOrder = item.SortOrder,
                AddedAt = item.AddedAt
            };
        }

        private PriceAlertDto MapToAlertDto(PriceAlert alert)
        {
            return new PriceAlertDto
            {
                Id = alert.Id,
                WatchlistId = alert.WatchlistId,
                Symbol = alert.Symbol,
                Type = alert.Type,
                TargetValue = alert.TargetValue,
                Status = alert.Status,
                CreatedAt = alert.CreatedAt,
                TriggeredAt = alert.TriggeredAt,
                Notes = alert.Notes
            };
        }
    }
}