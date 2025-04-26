using MaintenanceService.Domain.Enums;

namespace MaintenanceService.Domain.Models
{
    public class MaintenanceEvent
    {
        public Guid Id { get; private set; }
        public Guid VehicleId { get; private set; }
        public string Description { get; private set; }
        public MaintenanceType Type { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string PerformedBy { get; private set; }
        public double OdometerReading { get; private set; }
        public List<PartReplacement> PartsReplaced { get; private set; } = new List<PartReplacement>();
        public string Notes { get; private set; }

        public MaintenanceEvent(
            Guid vehicleId,
            string description,
            MaintenanceType type,
            string performedBy,
            double odometerReading,
            string notes = "")
        {
            Id = Guid.NewGuid();
            VehicleId = vehicleId;
            Description = description;
            Type = type;
            Timestamp = DateTime.UtcNow;
            PerformedBy = performedBy;
            OdometerReading = odometerReading;
            Notes = notes;
        }

        public void AddPartReplacement(string partId, string partName, int quantity, double cost)
        {
            PartsReplaced.Add(new PartReplacement(partId, partName, quantity, cost));
        }
    }
}
