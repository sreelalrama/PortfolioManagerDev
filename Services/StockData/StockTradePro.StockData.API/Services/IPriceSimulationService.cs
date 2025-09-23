using StockTradePro.StockData.API.Models.DTOs;

namespace StockTradePro.StockData.API.Services
{
    public interface IPriceSimulationService
    {
        Task<bool> IsMarketOpenAsync();
        Task UpdateAllStockPricesAsync();
        Task<StockPriceDto?> UpdateStockPriceAsync(string symbol);
        Task<List<StockDto>> GetUpdatedStocksAsync(DateTime since);
        TimeSpan GetTimeUntilMarketOpen();
        TimeSpan GetTimeUntilMarketClose();
    }
}