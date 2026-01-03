
using StockTradePro.WatchlistService.API.DTOs;

namespace StockTradePro.WatchlistService.API.Services
{
    public interface IStockDataService
    {
        Task<StockDto?> GetStockAsync(string symbol);
        Task<StockPriceDto?> GetCurrentPriceAsync(string symbol);
    }

    public class StockDto
    {
        public string Symbol { get; set; }
        public string CompanyName { get; set; }
    }
}