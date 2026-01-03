using StockTradePro.WatchlistService.API.DTOs;

namespace StockTradePro.WatchlistService.API.Services
{
    public interface IPriceAlertService
    {
        Task<IEnumerable<PriceAlertDto>> GetWatchlistAlertsAsync(int watchlistId, string userId);
        Task<PriceAlertDto?> GetAlertAsync(int id, string userId);
        Task<PriceAlertDto?> CreateAlertAsync(int watchlistId, CreatePriceAlertDto dto, string userId);
        Task<bool> DeleteAlertAsync(int id, string userId);
        Task<bool> DisableAlertAsync(int id, string userId);
        Task<IEnumerable<PriceAlertDto>> GetActiveAlertsAsync();
        Task<bool> TriggerAlertAsync(int alertId);
    }
}
