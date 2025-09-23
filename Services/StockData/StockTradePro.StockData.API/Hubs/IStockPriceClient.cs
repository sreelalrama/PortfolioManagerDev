
namespace StockTradePro.StockData.API.Hubs
{
    public interface IStockPriceClient
    {
        Task PriceUpdate(StockPriceUpdateDto update);
        Task BulkPriceUpdate(List<StockPriceUpdateDto> updates);
        Task MarketStatus(object status);
        Task SubscriptionConfirmed(List<string> symbols);
        Task UnsubscriptionConfirmed(List<string> symbols);
        Task AllStocksSubscriptionConfirmed();
        Task AllStocksUnsubscriptionConfirmed();
        Task Error(string message);
    }
}