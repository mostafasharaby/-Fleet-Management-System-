using MaintenanceService.Domain.Enums;
using MaintenanceService.Domain.Events;

namespace MaintenanceService.Domain.Models
{
    public class MaintenanceTask
    {
        public Guid Id { get; private set; }
        public Guid VehicleId { get; private set; }
        public string Description { get; private set; }
        public MaintenanceType Type { get; private set; }
        public DateTime ScheduledDate { get; private set; }
        public MaintenanceStatus Status { get; private set; }
        public Guid? AssignedTechnicianId { get; private set; }
        public int EstimatedDurationMinutes { get; private set; }
        public List<RequiredPart> RequiredParts { get; private set; } = new List<RequiredPart>();
        public DateTime CreatedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }

        // Domain events for when status changes
        public List<MaintenanceDomainEvent> DomainEvents { get; private set; } = new List<MaintenanceDomainEvent>();

        public MaintenanceTask(
            Guid vehicleId,
            string description,
            MaintenanceType type,
            DateTime scheduledDate,
            Guid? assignedTechnicianId = null,
            int estimatedDurationMinutes = 60)
        {
            Id = Guid.NewGuid();
            VehicleId = vehicleId;
            Description = description;
            Type = type;
            ScheduledDate = scheduledDate;
            Status = MaintenanceStatus.Scheduled;
            AssignedTechnicianId = assignedTechnicianId;
            EstimatedDurationMinutes = estimatedDurationMinutes;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateStatus(MaintenanceStatus newStatus, string updatedBy)
        {
            var oldStatus = Status;
            Status = newStatus;

            if (newStatus == MaintenanceStatus.Completed)
            {
                CompletedAt = DateTime.UtcNow;
            }

            // Add domain event
            DomainEvents.Add(new MaintenanceStatusChangedEvent(
                Id, VehicleId, oldStatus, newStatus, updatedBy, DateTime.UtcNow));
        }

        public void AddRequiredPart(string partId, string partName, int quantity)
        {
            RequiredParts.Add(new RequiredPart("", partId, partName, quantity));
        }

        public void Reschedule(DateTime newScheduledDate)
        {
            if (Status == MaintenanceStatus.Completed || Status == MaintenanceStatus.Canceled)
            {
                throw new InvalidOperationException("Cannot reschedule completed or canceled maintenance tasks");
            }

            ScheduledDate = newScheduledDate;
        }

        public void AssignTechnician(Guid technicianId)
        {
            AssignedTechnicianId = technicianId;
        }
    }
}
