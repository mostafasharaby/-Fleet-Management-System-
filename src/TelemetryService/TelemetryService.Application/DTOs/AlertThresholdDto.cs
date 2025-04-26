using TelemetryService.Domain.Enums;

namespace TelemetryService.Application.DTOs
{
    public class AlertThresholdDto
    {
        public Guid Id { get; set; }
        public Guid VehicleId { get; set; }
        public string ParameterName { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public bool IsEnabled { get; set; }
        public string AlertMessage { get; set; }
        public AlertSeverity Severity { get; set; }
    }
}
