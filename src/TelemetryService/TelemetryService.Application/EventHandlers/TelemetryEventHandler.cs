using MediatR;
using Microsoft.Extensions.Logging;
using TelemetryService.Domain.Events;
using TelemetryService.Domain.Messaging;
namespace TelemetryService.Application.EventHandlers
{
    public class TelemetryEventHandler :
       INotificationHandler<TelemetryDataReceivedEvent>,
       INotificationHandler<ThresholdAlertEvent>
    {
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILogger<TelemetryEventHandler> _logger;

        public TelemetryEventHandler(
            IMessagePublisher messagePublisher,
            ILogger<TelemetryEventHandler> logger)
        {
            _messagePublisher = messagePublisher;
            _logger = logger;
        }

        public async Task Handle(TelemetryDataReceivedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling telemetry data received event for vehicle {VehicleId}", notification.VehicleId);
            await _messagePublisher.PublishAsync("telemetry.data.received", notification);
        }

        public async Task Handle(ThresholdAlertEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogWarning("Threshold alert triggered for vehicle {VehicleId}: {ParameterName} = {ParameterValue}, Severity: {Severity}",
                notification.VehicleId, notification.ParameterName, notification.ParameterValue, notification.Severity);

            // Publish to notification service
            await _messagePublisher.PublishAsync("telemetry.threshold.alert", notification);
        }
    }
}
