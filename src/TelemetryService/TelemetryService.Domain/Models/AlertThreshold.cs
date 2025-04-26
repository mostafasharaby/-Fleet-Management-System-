using TelemetryService.Domain.Enums;

namespace TelemetryService.Domain.Models
{
    public class AlertThreshold
    {
        public Guid Id { get; private set; }
        public Guid VehicleId { get; private set; }
        public string ParameterName { get; private set; }
        public double MinValue { get; private set; }
        public double MaxValue { get; private set; }
        public bool IsEnabled { get; private set; }
        public string AlertMessage { get; private set; }
        public AlertSeverity Severity { get; private set; }

        private AlertThreshold() { }

        public AlertThreshold(Guid vehicleId, string parameterName, double minValue, double maxValue,
            bool isEnabled, string alertMessage, AlertSeverity severity)
        {
            Id = Guid.NewGuid();
            VehicleId = vehicleId;
            ParameterName = parameterName;
            MinValue = minValue;
            MaxValue = maxValue;
            IsEnabled = isEnabled;
            AlertMessage = alertMessage;
            Severity = severity;
        }
    }

}
