namespace VehicleService.Domain.Models
{
    public class MaintenanceRecord
    {
        public Guid Id { get; set; }
        public Guid VehicleId { get; set; }
        public string Description { get; set; }
        public string Technician { get; set; }
        public float Cost { get; set; }
        public float OdometerReading { get; set; }
        public DateTime CompletedDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
