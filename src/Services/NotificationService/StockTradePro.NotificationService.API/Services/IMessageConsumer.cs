namespace StockTradePro.NotificationService.API.Services
{
    public interface IMessageConsumer : IDisposable
    {
        Task StartConsumingAsync();
        Task StopConsumingAsync();
    }
}