using Microsoft.Extensions.Logging;
using NotificationService.Domain.Enums;
using System.Text.Json;

namespace NotificationService.Infrastructure.Messaging
{
    public class VehicleEventConsumer : EventConsumerBase
    {
        public VehicleEventConsumer(
            RabbitMQClient rabbitMQClient,
            INotificationService notificationService,
            ILogger<VehicleEventConsumer> logger)
            : base(rabbitMQClient, notificationService, logger)
        {
        }

        protected override string QueueName => "notification-service.vehicle-events";
        protected override string ExchangeName => "fleet-management.vehicle-events";
        protected override string RoutingKey => "vehicle.#";

        protected override async Task ProcessMessageAsync(string message)
        {
            try
            {
                var eventData = JsonSerializer.Deserialize<VehicleEvent>(message);

                if (eventData == null)
                {
                    _logger.LogWarning("Received null vehicle event data");
                    return;
                }

                switch (eventData.EventType)
                {
                    case "vehicle.alert":
                        await ProcessVehicleAlertAsync(eventData);
                        break;
                    case "vehicle.maintenance":
                        await ProcessVehicleMaintenanceAsync(eventData);
                        break;
                    default:
                        _logger.LogWarning($"Received unknown vehicle event type: {eventData.EventType}");
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing vehicle event message");
            }
        }

        private async Task ProcessVehicleAlertAsync(VehicleEvent eventData)
        {
            var metadata = new Dictionary<string, string>
            {
                { "vehicleId", eventData.VehicleId },
                { "alertType", eventData.Data.ContainsKey("alertType") ? eventData.Data["alertType"] : "Unknown" },
                { "severity", eventData.Data.ContainsKey("severity") ? eventData.Data["severity"] : "Medium" }
            };

            var priority = eventData.Data.ContainsKey("severity") && eventData.Data["severity"] == "High"
                ? NotificationPriority.Urgent
                : NotificationPriority.High;

            await _notificationService.SendNotificationAsync(
                eventData.Data.ContainsKey("fleetManagerId") ? eventData.Data["fleetManagerId"] : "admin",
                NotificationType.VehicleAlert,
                priority,
                $"Vehicle Alert: {eventData.VehicleId}",
                eventData.Data.ContainsKey("message") ? eventData.Data["message"] : "Vehicle alert detected",
                metadata,
                new List<NotificationChannel> { NotificationChannel.Email, NotificationChannel.InApp });
        }

        private async Task ProcessVehicleMaintenanceAsync(VehicleEvent eventData)
        {
            var metadata = new Dictionary<string, string>
            {
                { "vehicleId", eventData.VehicleId },
                { "maintenanceType", eventData.Data.ContainsKey("maintenanceType") ? eventData.Data["maintenanceType"] : "Regular" },
                { "scheduledDate", eventData.Data.ContainsKey("scheduledDate") ? eventData.Data["scheduledDate"] : DateTime.UtcNow.AddDays(7).ToString("yyyy-MM-dd") }
            };

            await _notificationService.SendNotificationAsync(
                eventData.Data.ContainsKey("fleetManagerId") ? eventData.Data["fleetManagerId"] : "admin",
                NotificationType.MaintenanceReminder,
                NotificationPriority.Normal,
                $"Maintenance Reminder: {eventData.VehicleId}",
                $"Vehicle {eventData.VehicleId} is due for {metadata["maintenanceType"]} maintenance on {metadata["scheduledDate"]}",
                metadata,
                new List<NotificationChannel> { NotificationChannel.Email, NotificationChannel.InApp });
        }
    }

    public class VehicleEvent
    {
        public string EventType { get; set; }
        public string VehicleId { get; set; }
        public Dictionary<string, string> Data { get; set; }
    }
}
