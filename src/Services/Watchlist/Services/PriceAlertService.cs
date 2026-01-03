
using Microsoft.EntityFrameworkCore;
using StockTradePro.WatchlistService.API.Data;
using StockTradePro.WatchlistService.API.DTOs;
using StockTradePro.WatchlistService.API.Models;

namespace StockTradePro.WatchlistService.API.Services
{
    public class PriceAlertService : IPriceAlertService
    {
        private readonly WatchlistDbContext _context;
        private readonly IStockDataService _stockDataService;
        private readonly ILogger<PriceAlertService> _logger;

        public PriceAlertService(
            WatchlistDbContext context,
            IStockDataService stockDataService,
            ILogger<PriceAlertService> logger)
        {
            _context = context;
            _stockDataService = stockDataService;
            _logger = logger;
        }

        public async Task<IEnumerable<PriceAlertDto>> GetWatchlistAlertsAsync(int watchlistId, string userId)
        {
            var alerts = await _context.PriceAlerts
                .Where(a => a.WatchlistId == watchlistId)
                .Include(a => a.Watchlist)
                .Where(a => a.Watchlist.UserId == userId)
                .OrderBy(a => a.CreatedAt)
                .ToListAsync();

            return alerts.Select(MapToDto);
        }

        public async Task<PriceAlertDto?> GetAlertAsync(int id, string userId)
        {
            var alert = await _context.PriceAlerts
                .Where(a => a.Id == id)
                .Include(a => a.Watchlist)
                .Where(a => a.Watchlist.UserId == userId)
                .FirstOrDefaultAsync();

            return alert != null ? MapToDto(alert) : null;
        }

        public async Task<PriceAlertDto?> CreateAlertAsync(int watchlistId, CreatePriceAlertDto dto, string userId)
        {
            var watchlist = await _context.Watchlists
                .Where(w => w.Id == watchlistId && w.UserId == userId)
                .FirstOrDefaultAsync();

            if (watchlist == null) return null;

            // Verify stock exists
            var stockExists = await _stockDataService.GetStockAsync(dto.Symbol);
            if (stockExists == null)
            {
                _logger.LogWarning("Attempted to create alert for non-existent stock {Symbol}", dto.Symbol);
                return null;
            }

            var alert = new PriceAlert
            {
                WatchlistId = watchlistId,
                Symbol = dto.Symbol.ToUpper(),
                Type = dto.Type,
                TargetValue = dto.TargetValue,
                Status = AlertStatus.Active,
                CreatedAt = DateTime.UtcNow,
                Notes = dto.Notes
            };

            _context.PriceAlerts.Add(alert);
            await _context.SaveChangesAsync();

            return MapToDto(alert);
        }

        public async Task<bool> DeleteAlertAsync(int id, string userId)
        {
            var alert = await _context.PriceAlerts
                .Where(a => a.Id == id)
                .Include(a => a.Watchlist)
                .Where(a => a.Watchlist.UserId == userId)
                .FirstOrDefaultAsync();

            if (alert == null) return false;

            _context.PriceAlerts.Remove(alert);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DisableAlertAsync(int id, string userId)
        {
            var alert = await _context.PriceAlerts
                .Where(a => a.Id == id)
                .Include(a => a.Watchlist)
                .Where(a => a.Watchlist.UserId == userId)
                .FirstOrDefaultAsync();

            if (alert == null) return false;

            alert.Status = AlertStatus.Disabled;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<PriceAlertDto>> GetActiveAlertsAsync()
        {
            var alerts = await _context.PriceAlerts
                .Where(a => a.Status == AlertStatus.Active)
                .Include(a => a.Watchlist)
                .ToListAsync();

            return alerts.Select(MapToDto);
        }

        public async Task<bool> TriggerAlertAsync(int alertId)
        {
            var alert = await _context.PriceAlerts
                .Where(a => a.Id == alertId)
                .FirstOrDefaultAsync();

            if (alert == null) return false;

            alert.Status = AlertStatus.Triggered;
            alert.TriggeredAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        private PriceAlertDto MapToDto(PriceAlert alert)
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