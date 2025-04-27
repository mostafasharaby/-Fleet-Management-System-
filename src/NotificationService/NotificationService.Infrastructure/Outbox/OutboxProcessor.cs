using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NotificationService.Infrastructure.Data;
using NotificationService.Infrastructure.Messaging;

namespace NotificationService.Infrastructure.Outbox
{
    public class OutboxProcessor : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OutboxProcessor> _logger;
        private readonly TimeSpan _processInterval = TimeSpan.FromSeconds(10);

        public OutboxProcessor(
            IServiceProvider serviceProvider,
            ILogger<OutboxProcessor> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Outbox processor started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessOutboxMessagesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing outbox messages");
                }

                await Task.Delay(_processInterval, stoppingToken);
            }

            _logger.LogInformation("Outbox processor stopped");
        }

        private async Task ProcessOutboxMessagesAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
            var rabbitMQClient = scope.ServiceProvider.GetRequiredService<RabbitMQClient>();

            var messages = await dbContext.OutboxMessages
                .Where(m => m.ProcessedAt == null)
                .OrderBy(m => m.CreatedAt)
                .Take(20)
                .ToListAsync(stoppingToken);

            if (!messages.Any())
            {
                return;
            }

            _logger.LogInformation($"Processing {messages.Count} outbox messages");

            foreach (var message in messages)
            {
                try
                {
                    // Extract event type to determine exchange and routing key
                    var parts = message.EventType.Split('.');
                    var exchange = $"fleet-management.{parts[0]}-events";
                    var routingKey = message.EventType;

                    rabbitMQClient.DeclareExchange(exchange);
                    rabbitMQClient.Publish(exchange, routingKey, message.Content);

                    message.ProcessedAt = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    message.Error = ex.Message;
                    _logger.LogError(ex, $"Failed to process outbox message {message.Id}");
                }
            }

            await dbContext.SaveChangesAsync(stoppingToken);
        }
    }
}
