namespace VehicleService.Domain.Models
{
    public class FuelRecord
    {
        public Guid Id { get; set; }
        public Guid VehicleId { get; set; }
        public float Amount { get; set; }
        public float Cost { get; set; }
        public string Station { get; set; }
        public string DriverName { get; set; }
        public float OdometerReading { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
