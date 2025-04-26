using MaintenanceService.Domain.Enums;

namespace MaintenanceService.Domain.Events
{
    public class MaintenanceStatusChangedEvent : MaintenanceDomainEvent
    {
        public Guid MaintenanceTaskId { get; }
        public Guid VehicleId { get; }
        public MaintenanceStatus OldStatus { get; }
        public MaintenanceStatus NewStatus { get; }
        public string UpdatedBy { get; }
        public DateTime UpdatedAt { get; }

        public MaintenanceStatusChangedEvent(
            Guid maintenanceTaskId,
            Guid vehicleId,
            MaintenanceStatus oldStatus,
            MaintenanceStatus newStatus,
            string updatedBy,
            DateTime updatedAt)
        {
            MaintenanceTaskId = maintenanceTaskId;
            VehicleId = vehicleId;
            OldStatus = oldStatus;
            NewStatus = newStatus;
            UpdatedBy = updatedBy;
            UpdatedAt = updatedAt;
        }
    }
}
