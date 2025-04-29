using Microsoft.Extensions.Logging;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Enums;
using System.Text.Json;

namespace NotificationService.Infrastructure.Messaging
{
    public class DriverEventConsumer : EventConsumerBase
    {
        public DriverEventConsumer(
            RabbitMQClient rabbitMQClient,
            INotificationService notificationService,
            ILogger<DriverEventConsumer> logger)
            : base(rabbitMQClient, notificationService, logger)
        {
        }

        protected override string QueueName => "notification-service.driver-events";
        protected override string ExchangeName => "fleet-management.driver-events";
        protected override string RoutingKey => "driver.#";

        protected override async Task ProcessMessageAsync(string message)
        {
            try
            {
                var eventData = JsonSerializer.Deserialize<DriverEvent>(message);

                if (eventData == null)
                {
                    _logger.LogWarning("Received null driver event data");
                    return;
                }

                switch (eventData.EventType)
                {
                    case "driver.assignment":
                        await ProcessDriverAssignmentAsync(eventData);
                        break;
                    case "driver.route_update":
                        await ProcessRouteUpdateAsync(eventData);
                        break;
                    default:
                        _logger.LogWarning($"Received unknown driver event type: {eventData.EventType}");
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing driver event message");
            }
        }

        private async Task ProcessDriverAssignmentAsync(DriverEvent eventData)
        {
            var metadata = new Dictionary<string, string>
            {
                { "driverId", eventData.DriverId },
                { "vehicleId", eventData.Data.ContainsKey("vehicleId") ? eventData.Data["vehicleId"] : "Unknown" },
                { "assignmentDate", eventData.Data.ContainsKey("assignmentDate") ? eventData.Data["assignmentDate"] : DateTime.UtcNow.ToString("yyyy-MM-dd") }
            };

            await _notificationService.SendNotificationAsync(
                eventData.DriverId,
                NotificationType.DriverAssignment,
                NotificationPriority.Normal,
                "New Vehicle Assignment",
                $"You have been assigned vehicle {metadata["vehicleId"]} starting {metadata["assignmentDate"]}",
                metadata,
                new List<NotificationChannel> { NotificationChannel.Email, NotificationChannel.SMS, NotificationChannel.InApp });
        }

        private async Task ProcessRouteUpdateAsync(DriverEvent eventData)
        {
            var metadata = new Dictionary<string, string>
            {
                { "driverId", eventData.DriverId },
                { "routeId", eventData.Data.ContainsKey("routeId") ? eventData.Data["routeId"] : "Unknown" },
                { "updateType", eventData.Data.ContainsKey("updateType") ? eventData.Data["updateType"] : "Change" }
            };

            await _notificationService.SendNotificationAsync(
                eventData.DriverId,
                NotificationType.RouteUpdate,
                NotificationPriority.High,
                "Route Update",
                eventData.Data.ContainsKey("message") ? eventData.Data["message"] : "Your route has been updated",
                metadata,
                new List<NotificationChannel> { NotificationChannel.Push, NotificationChannel.SMS, NotificationChannel.InApp });
        }
    }

    public class DriverEvent
    {
        public string EventType { get; set; }
        public string DriverId { get; set; }
        public Dictionary<string, string> Data { get; set; }
    }
}
