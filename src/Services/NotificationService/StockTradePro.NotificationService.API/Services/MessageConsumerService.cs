using StockTradePro.NotificationService.API.Services;

namespace StockTradePro.NotificationService.API.BackgroundServices
{
    public class MessageConsumerService : BackgroundService
    {
        private readonly IMessageConsumer _messageConsumer;
        private readonly ILogger<MessageConsumerService> _logger;

        public MessageConsumerService(IMessageConsumer messageConsumer, ILogger<MessageConsumerService> logger)
        {
            _messageConsumer = messageConsumer;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Message Consumer Service starting");

            await _messageConsumer.StartConsumingAsync();

            // Keep the service running
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }

            await _messageConsumer.StopConsumingAsync();
            _logger.LogInformation("Message Consumer Service stopped");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _messageConsumer.StopConsumingAsync();
            await base.StopAsync(cancellationToken);
        }
    }
}