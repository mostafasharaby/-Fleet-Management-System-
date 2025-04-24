using DriverService.Domain.Enums;

namespace DriverService.Domain.Models
{
    public class Assignment
    {
        public Guid Id { get; set; }
        public Guid DriverId { get; set; }
        public Guid VehicleId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Notes { get; set; }
        public AssignmentStatus Status { get; set; }
        public float StartOdometer { get; set; }
        public float? EndOdometer { get; set; }
        public float? FuelLevel { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
