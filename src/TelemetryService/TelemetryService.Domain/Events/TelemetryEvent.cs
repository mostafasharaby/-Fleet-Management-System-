namespace TelemetryService.Domain.Events
{
    public abstract class TelemetryEvent : MediatR.INotification
    {
        public Guid Id { get; protected set; }
        public DateTime Timestamp { get; protected set; }
        public Guid VehicleId { get; protected set; }

        protected TelemetryEvent(Guid vehicleId)
        {
            Id = Guid.NewGuid();
            Timestamp = DateTime.UtcNow;
            VehicleId = vehicleId;
        }
    }
}
