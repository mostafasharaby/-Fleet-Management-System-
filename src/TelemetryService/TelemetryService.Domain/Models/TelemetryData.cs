namespace TelemetryService.Domain.Models
{
    public class TelemetryData
    {
        public Guid Id { get; private set; }
        public Guid VehicleId { get; private set; }
        public DateTime Timestamp { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public double Speed { get; private set; }
        public double FuelLevel { get; private set; }
        public double EngineTemperature { get; private set; }
        public double BatteryVoltage { get; private set; }
        public int EngineRpm { get; private set; }
        public bool CheckEngineLightOn { get; private set; }
        public double OdometerReading { get; private set; }
        public string DiagnosticCode { get; private set; }

        private TelemetryData() { }

        public TelemetryData(Guid vehicleId, DateTime timestamp, double latitude, double longitude,
            double speed, double fuelLevel, double engineTemperature, double batteryVoltage,
            int engineRpm, bool checkEngineLightOn, double odometerReading, string diagnosticCode = null)
        {
            Id = Guid.NewGuid();
            VehicleId = vehicleId;
            Timestamp = timestamp;
            Latitude = latitude;
            Longitude = longitude;
            Speed = speed;
            FuelLevel = fuelLevel;
            EngineTemperature = engineTemperature;
            BatteryVoltage = batteryVoltage;
            EngineRpm = engineRpm;
            CheckEngineLightOn = checkEngineLightOn;
            OdometerReading = odometerReading;
            DiagnosticCode = diagnosticCode;
        }
    }
}

