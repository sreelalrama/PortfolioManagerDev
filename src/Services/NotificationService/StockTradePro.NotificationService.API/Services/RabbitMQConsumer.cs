using RabbitMQ.Client;
using StockTradePro.NotificationService.API.DTOs;
using StockTradePro.Shared.Messages;
using StockTradePro.Shared.Messaging;
using System.Text;
using System.Text.Json;

namespace StockTradePro.NotificationService.API.Services
{
    public class RabbitMQConsumer : IMessageConsumer
    {
        private RabbitMQ.Client.IConnection _connection;
        private RabbitMQ.Client.IModel _channel;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RabbitMQConsumer> _logger;
        private readonly IConfiguration _configuration;

        public RabbitMQConsumer(IConfiguration configuration, IServiceProvider serviceProvider, ILogger<RabbitMQConsumer> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;
        }

        private void InitializeRabbitMQ()
        {
            var rabbitMQSettings = _configuration.GetSection("RabbitMQ").Get<RabbitMQSettings>();

            var factory = new RabbitMQ.Client.ConnectionFactory()
            {
                HostName = rabbitMQSettings.HostName,
                Port = rabbitMQSettings.Port,
                UserName = rabbitMQSettings.UserName,
                Password = rabbitMQSettings.Password,
                VirtualHost = rabbitMQSettings.VirtualHost,
                DispatchConsumersAsync = true
            };

            var retryCount = 0;
            while (_connection == null && retryCount < 10)
            {
                try
                {
                    _connection = factory.CreateConnection();
                    _channel = _connection.CreateModel();

                    // Declare exchange
                    _channel.ExchangeDeclare(
                        exchange: ExchangeNames.Notifications,
                        type: RabbitMQ.Client.ExchangeType.Topic,
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

                    // Set QoS to process one message at a time
                    _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                    
                    _logger.LogInformation("Connected to RabbitMQ successfully");
                }
                catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException)
                {
                    retryCount++;
                    var delay = TimeSpan.FromSeconds(Math.Pow(2, retryCount));
                    _logger.LogWarning("RabbitMQ not reachable. Retrying in {Delay} seconds...", delay.TotalSeconds);
                    Thread.Sleep(delay);
                }
            }

            if (_connection == null)
            {
                throw new Exception("Could not connect to RabbitMQ after multiple retries.");
            }
        }

        public async Task StartConsumingAsync()
        {
            InitializeRabbitMQ();

            // Start consuming price alerts
            var priceAlertConsumer = new RabbitMQ.Client.Events.EventingBasicConsumer(_channel);
            priceAlertConsumer.Received += async (model, ea) =>
            {
                await HandlePriceAlertMessage(ea);
            };

            _channel.BasicConsume(
                queue: QueueNames.PriceAlerts,
                autoAck: false,
                consumer: priceAlertConsumer);

            // Start consuming watchlist updates
            var watchlistUpdateConsumer = new RabbitMQ.Client.Events.EventingBasicConsumer(_channel);
            watchlistUpdateConsumer.Received += async (model, ea) =>
            {
                await HandleWatchlistUpdateMessage(ea);
            };

            _channel.BasicConsume(
                queue: QueueNames.WatchlistUpdates,
                autoAck: false,
                consumer: watchlistUpdateConsumer);

            _logger.LogInformation("RabbitMQ Consumer started consuming messages");
            await Task.CompletedTask;
        }

        public async Task StopConsumingAsync()
        {
            _channel?.Close();
            _logger.LogInformation("RabbitMQ Consumer stopped");
            await Task.CompletedTask;
        }

        private async Task HandlePriceAlertMessage(RabbitMQ.Client.Events.BasicDeliverEventArgs ea)
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var priceAlert = JsonSerializer.Deserialize<PriceAlertMessage>(message);

                if (priceAlert == null)
                {
                    _logger.LogWarning("Failed to deserialize price alert message");
                    _channel.BasicNack(ea.DeliveryTag, false, false);
                    return;
                }

                // Process the price alert notification
                using var scope = _serviceProvider.CreateScope();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                var request = new PriceAlertRequestDto
                {
                    UserId = priceAlert.UserId,
                    AlertData = new PriceAlertNotificationDto
                    {
                        AlertId = priceAlert.AlertId,
                        Symbol = priceAlert.Symbol,
                        AlertType = priceAlert.AlertType,
                        TargetValue = priceAlert.TargetValue,
                        CurrentPrice = priceAlert.CurrentPrice,
                        ChangePercent = priceAlert.ChangePercent
                    }
                };

                await notificationService.SendPriceAlertNotificationAsync(request);

                // Acknowledge the message
                _channel.BasicAck(ea.DeliveryTag, false);

                _logger.LogInformation("Processed price alert notification for user {UserId}, symbol {Symbol}",
                    priceAlert.UserId, priceAlert.Symbol);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing price alert message");

                // Reject the message and requeue for retry
                _channel.BasicNack(ea.DeliveryTag, false, true);
            }
        }

        private async Task HandleWatchlistUpdateMessage(RabbitMQ.Client.Events.BasicDeliverEventArgs ea)
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var watchlistUpdate = JsonSerializer.Deserialize<WatchlistUpdateMessage>(message);

                if (watchlistUpdate == null)
                {
                    _logger.LogWarning("Failed to deserialize watchlist update message");
                    _channel.BasicNack(ea.DeliveryTag, false, false);
                    return;
                }

                // Process the watchlist update notification
                using var scope = _serviceProvider.CreateScope();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                var title = $"Watchlist Update: {watchlistUpdate.WatchlistName}";
                var message_text = watchlistUpdate.Action switch
                {
                    "Added" => $"Added {watchlistUpdate.Symbol} to your watchlist {watchlistUpdate.WatchlistName}",
                    "Removed" => $"Removed {watchlistUpdate.Symbol} from your watchlist {watchlistUpdate.WatchlistName}",
                    "Created" => $"Created new watchlist: {watchlistUpdate.WatchlistName}",
                    "Deleted" => $"Deleted watchlist: {watchlistUpdate.WatchlistName}",
                    _ => $"Updated your watchlist: {watchlistUpdate.WatchlistName}"
                };

                var notificationDto = new CreateNotificationDto
                {
                    UserId = watchlistUpdate.UserId,
                    Type = Models.NotificationType.WatchlistUpdate,
                    Title = title,
                    Message = message_text,
                    Data = JsonSerializer.Serialize(watchlistUpdate)
                };

                await notificationService.CreateNotificationAsync(notificationDto);

                // Acknowledge the message
                _channel.BasicAck(ea.DeliveryTag, false);

                _logger.LogInformation("Processed watchlist update notification for user {UserId}: {Action}",
                    watchlistUpdate.UserId, watchlistUpdate.Action);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing watchlist update message");

                // Reject the message and requeue for retry
                _channel.BasicNack(ea.DeliveryTag, false, true);
            }
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}