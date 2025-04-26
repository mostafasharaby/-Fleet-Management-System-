namespace TelemetryService.Application.DTOs
{
    public class TelemetryDataDto
    {
        public Guid Id { get; set; }
        public Guid VehicleId { get; set; }
        public DateTime Timestamp { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Speed { get; set; }
        public double FuelLevel { get; set; }
        public double EngineTemperature { get; set; }
        public double BatteryVoltage { get; set; }
        public int EngineRpm { get; set; }
        public bool CheckEngineLightOn { get; set; }
        public double OdometerReading { get; set; }
        public string DiagnosticCode { get; set; }
    }
}
