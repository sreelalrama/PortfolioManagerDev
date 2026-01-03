using StockTradePro.Portfolio.API.Models.DTOs;

namespace StockTradePro.Portfolio.API.Services
{
    public interface IStockDataService
    {
        Task<decimal?> GetCurrentStockPriceAsync(string symbol);
        Task<Dictionary<string, decimal>> GetMultipleStockPricesAsync(List<string> symbols);
        Task<StockInfoDto?> GetStockInfoAsync(string symbol);
    }
}