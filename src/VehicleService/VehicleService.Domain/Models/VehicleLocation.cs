namespace VehicleService.Domain.Models
{
    public class VehicleLocation
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Speed { get; set; }
        public double Heading { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
