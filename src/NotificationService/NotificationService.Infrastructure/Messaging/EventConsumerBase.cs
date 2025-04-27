using Microsoft.Extensions.Logging;

namespace NotificationService.Infrastructure.Messaging
{
    public abstract class EventConsumerBase : BackgroundService
    {
        protected readonly RabbitMQClient _rabbitMQClient;
        protected readonly ILogger _logger;
        protected readonly INotificationService _notificationService;

        protected EventConsumerBase(
            RabbitMQClient rabbitMQClient,
            INotificationService notificationService,
            ILogger logger)
        {
            _rabbitMQClient = rabbitMQClient ?? throw new ArgumentNullException(nameof(rabbitMQClient));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected abstract string QueueName { get; }
        protected abstract string ExchangeName { get; }
        protected abstract string RoutingKey { get; }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _rabbitMQClient.DeclareExchange(ExchangeName);
            _rabbitMQClient.DeclareQueue(QueueName);
            _rabbitMQClient.BindQueue(QueueName, ExchangeName, RoutingKey);
            _rabbitMQClient.Subscribe(QueueName, ProcessMessageAsync);

            return Task.CompletedTask;
        }

        protected abstract Task ProcessMessageAsync(string message);
    }
}
