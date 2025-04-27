using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace NotificationService.Infrastructure.Messaging
{
    public class RabbitMQClient : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger<RabbitMQClient> _logger;

        public RabbitMQClient(IOptions<RabbitMQSettings> settings, ILogger<RabbitMQClient> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var rabbitMQSettings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));

            var factory = new ConnectionFactory
            {
                HostName = rabbitMQSettings.HostName,
                Port = rabbitMQSettings.Port,
                UserName = rabbitMQSettings.UserName,
                Password = rabbitMQSettings.Password,
                VirtualHost = rabbitMQSettings.VirtualHost,
                DispatchConsumersAsync = true
            };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _logger.LogInformation("Successfully connected to RabbitMQ");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to RabbitMQ");
                throw;
            }
        }

        public void DeclareExchange(string exchangeName, string exchangeType = ExchangeType.Topic)
        {
            _channel.ExchangeDeclare(exchangeName, exchangeType, durable: true);
        }

        public void DeclareQueue(string queueName, bool durable = true)
        {
            _channel.QueueDeclare(queueName, durable, false, false, null);
        }

        public void BindQueue(string queueName, string exchangeName, string routingKey)
        {
            _channel.QueueBind(queueName, exchangeName, routingKey);
        }

        public void Publish(string exchangeName, string routingKey, string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(
                exchange: exchangeName,
                routingKey: routingKey,
                basicProperties: null,
                body: body);

            _logger.LogInformation($"Published message to exchange {exchangeName} with routing key {routingKey}");
        }

        public void Subscribe(string queueName, Func<string, Task> messageHandler)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {
                    await messageHandler(message);
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing message from queue {queueName}");
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };

            _channel.BasicConsume(queueName, false, consumer);
            _logger.LogInformation($"Subscribed to queue {queueName}");
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
    public class RabbitMQSettings
    {
        public string HostName { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
    }
}
