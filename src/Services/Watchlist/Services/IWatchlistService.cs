using StockTradePro.WatchlistService.API.Data;
using StockTradePro.WatchlistService.API.DTOs;
using StockTradePro.WatchlistService.API.Models;

namespace StockTradePro.WatchlistService.API.Services
{
    public interface IWatchlistService
    {
        Task<IEnumerable<WatchlistDto>> GetUserWatchlistsAsync(string userId);
        Task<WatchlistDto?> GetWatchlistAsync(int id, string userId);
        Task<WatchlistDto> CreateWatchlistAsync(CreateWatchlistDto dto, string userId);
        Task<WatchlistDto?> UpdateWatchlistAsync(int id, UpdateWatchlistDto dto, string userId);
        Task<bool> DeleteWatchlistAsync(int id, string userId);

        Task<WatchlistItemDto?> AddItemToWatchlistAsync(int watchlistId, AddWatchlistItemDto dto, string userId);
        Task<bool> RemoveItemFromWatchlistAsync(int watchlistId, int itemId, string userId);
        Task<bool> UpdateItemSortOrderAsync(int watchlistId, int itemId, int newSortOrder, string userId);

        Task<IEnumerable<WatchlistItemDto>> GetWatchlistItemsWithPricesAsync(int watchlistId, string userId);
    }
}
