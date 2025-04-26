using MaintenanceService.Domain.Enums;

namespace MaintenanceService.Domain.Events
{
    public class MaintenanceScheduledEvent : MaintenanceDomainEvent
    {
        public Guid MaintenanceTaskId { get; }
        public Guid VehicleId { get; }
        public DateTime ScheduledDate { get; }
        public MaintenanceType Type { get; }

        public MaintenanceScheduledEvent(
            Guid maintenanceTaskId,
            Guid vehicleId,
            DateTime scheduledDate,
            MaintenanceType type)
        {
            MaintenanceTaskId = maintenanceTaskId;
            VehicleId = vehicleId;
            ScheduledDate = scheduledDate;
            Type = type;
        }
    }
}
