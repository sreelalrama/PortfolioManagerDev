using StockTradePro.BlazorUI.Models.Watchlist;

namespace StockTradePro.BlazorUI.Services
{
    public interface IWatchlistService
    {
        Task<List<WatchlistDto>> GetWatchlistsAsync();
        Task<WatchlistDto?> GetWatchlistAsync(int id);
        Task<WatchlistDto?> CreateWatchlistAsync(CreateWatchlistDto createDto);
        Task<WatchlistDto?> UpdateWatchlistAsync(int id, UpdateWatchlistDto updateDto);
        Task<bool> DeleteWatchlistAsync(int id);
        Task<List<WatchlistItemDto>> GetWatchlistItemsAsync(int watchlistId);
        Task<WatchlistItemDto?> AddWatchlistItemAsync(int watchlistId, AddWatchlistItemDto addDto);
        Task<bool> DeleteWatchlistItemAsync(int watchlistId, int itemId);
        Task<bool> UpdateItemSortOrderAsync(int watchlistId, int itemId, int sortOrder);
        Task<List<PriceAlertDto>> GetPriceAlertsAsync(int watchlistId);
        Task<PriceAlertDto?> GetPriceAlertAsync(int watchlistId, int alertId);
        Task<PriceAlertDto?> CreatePriceAlertAsync(int watchlistId, CreatePriceAlertDto createDto);
        Task<bool> DeletePriceAlertAsync(int watchlistId, int alertId);
        Task<bool> DisablePriceAlertAsync(int watchlistId, int alertId);
        Task<List<PriceAlertDto>> GetActiveAlertsAsync();
    }
}
