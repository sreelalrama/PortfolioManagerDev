using StockTradePro.Shared.Messages;

namespace StockTradePro.WatchlistService.API.Services
{
    public interface IMessagePublisher
    {
        Task PublishPriceAlertAsync(PriceAlertMessage message);
        Task PublishWatchlistUpdateAsync(WatchlistUpdateMessage message);
    }
}