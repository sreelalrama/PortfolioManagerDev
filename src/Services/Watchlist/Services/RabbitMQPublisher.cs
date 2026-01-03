using RabbitMQ.Client;
using StockTradePro.Shared.Messages;
using StockTradePro.Shared.Messaging;
using System.Text;
using System.Text.Json;
// Remove: using RabbitMQ.Client;

namespace StockTradePro.WatchlistService.API.Services
{
    public class RabbitMQPublisher : IMessagePublisher, IDisposable
    {
        private readonly RabbitMQ.Client.IConnection _connection;
        private readonly RabbitMQ.Client.IModel _channel; // Fully qualified
        private readonly ILogger<RabbitMQPublisher> _logger;

        public RabbitMQPublisher(IConfiguration configuration, ILogger<RabbitMQPublisher> logger)
        {
            _logger = logger;

            var rabbitMQSettings = configuration.GetSection("RabbitMQ").Get<RabbitMQSettings>();

            var factory = new RabbitMQ.Client.ConnectionFactory() // Fully qualified
            {
                HostName = rabbitMQSettings.HostName,
                Port = rabbitMQSettings.Port,
                UserName = rabbitMQSettings.UserName,
                Password = rabbitMQSettings.Password,
                VirtualHost = rabbitMQSettings.VirtualHost
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declare exchange
            _channel.ExchangeDeclare(
                exchange: ExchangeNames.Notifications,
                type: RabbitMQ.Client.ExchangeType.Topic, // Fully qualified
                durable: true);

            // Declare queues
            _channel.QueueDeclare(
                queue: QueueNames.PriceAlerts,
                durable: true,
                exclusive: false,
                autoDelete: false);

            _channel.QueueDeclare(
                queue: QueueNames.WatchlistUpdates,
                durable: true,
                exclusive: false,
                autoDelete: false);

            // Bind queues to exchange
            _channel.QueueBind(
                queue: QueueNames.PriceAlerts,
                exchange: ExchangeNames.Notifications,
                routingKey: RoutingKeys.PriceAlert);

            _channel.QueueBind(
                queue: QueueNames.WatchlistUpdates,
                exchange: ExchangeNames.Notifications,
                routingKey: RoutingKeys.WatchlistUpdate);
        }

        public async Task PublishPriceAlertAsync(PriceAlertMessage message)
        {
            try
            {
                var json = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(json);

                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.MessageId = message.MessageId;
                properties.Timestamp = new RabbitMQ.Client.AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

                _channel.BasicPublish(
                    exchange: ExchangeNames.Notifications,
                    routingKey: RoutingKeys.PriceAlert,
                    basicProperties: properties,
                    body: body);

                _logger.LogInformation("Published price alert message for user {UserId}, symbol {Symbol}",
                    message.UserId, message.Symbol);

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish price alert message");
                throw;
            }
        }

        public async Task PublishWatchlistUpdateAsync(WatchlistUpdateMessage message)
        {
            try
            {
                var json = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(json);

                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.MessageId = message.MessageId;
                properties.Timestamp = new RabbitMQ.Client.AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

                _channel.BasicPublish(
                    exchange: ExchangeNames.Notifications,
                    routingKey: RoutingKeys.WatchlistUpdate,
                    basicProperties: properties,
                    body: body);

                _logger.LogInformation("Published watchlist update message for user {UserId}: {Action}",
                    message.UserId, message.Action);

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish watchlist update message");
                throw;
            }
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}