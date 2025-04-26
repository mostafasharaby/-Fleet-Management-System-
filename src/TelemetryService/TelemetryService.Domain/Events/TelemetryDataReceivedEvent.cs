namespace TelemetryService.Domain.Events
{
    public class TelemetryDataReceivedEvent : TelemetryEvent
    {
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public double Speed { get; private set; }

        public TelemetryDataReceivedEvent(Guid vehicleId, double latitude, double longitude, double speed)
            : base(vehicleId)
        {
            Latitude = latitude;
            Longitude = longitude;
            Speed = speed;
        }
    }
}
