using StockTradePro.StockData.API.Models.DTOs;

namespace StockTradePro.StockData.API.Services
{
    public interface IStockService
    {
        Task<PaginatedResult<StockDto>> GetStocksAsync(StockSearchDto searchDto);
        Task<StockDto?> GetStockBySymbolAsync(string symbol);
        Task<StockDto?> GetStockByIdAsync(int id);
        Task<List<StockPriceDto>> GetStockPricesAsync(string symbol, DateTime? fromDate = null, DateTime? toDate = null);
        Task<List<StockDto>> GetTrendingStocksAsync(int count = 10);
        Task<List<StockDto>> GetTopGainersAsync(int count = 10);
        Task<List<StockDto>> GetTopLosersAsync(int count = 10);
        Task<List<StockDto>> GetMostActiveAsync(int count = 10);
        Task<MarketOverviewDto> GetMarketOverviewAsync();
        Task<List<string>> GetSectorsAsync();
        Task<List<string>> GetExchangesAsync();
    }
}