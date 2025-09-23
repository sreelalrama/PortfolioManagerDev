using StockTradePro.BlazorUI.Models.Common;
using StockTradePro.BlazorUI.Models.Stocks;

namespace StockTradePro.BlazorUI.Services
{
    public interface IStockDataService
    {
        Task<MarketOverviewDto?> GetMarketOverviewAsync();
        Task<List<StockDto>> GetTrendingStocksAsync(int count = 10);
        Task<List<StockDto>> GetTopGainersAsync(int count = 10);
        Task<List<StockDto>> GetTopLosersAsync(int count = 10);
        Task<List<StockDto>> GetMostActiveAsync(int count = 10);
        Task<StockDto?> GetStockAsync(string symbol);
        Task<PaginatedResult<StockDto>> GetStocksAsync(StockSearchDto searchDto);
        Task<PaginatedResult<StockDto>> SearchStocksAsync(string query, int page = 1, int pageSize = 20);
        Task<List<string>> GetSectorsAsync();
        Task<List<string>> GetExchangesAsync();
        Task<List<StockPriceDto>> GetPricesAsync(string symbol, DateTime? fromDate = null, DateTime? toDate = null);
        Task<StockPriceDto?> GetCurrentPriceAsync(string symbol);
    }
}
