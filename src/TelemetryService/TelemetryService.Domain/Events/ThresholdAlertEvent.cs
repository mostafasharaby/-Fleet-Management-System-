using TelemetryService.Domain.Enums;
using TelemetryService.Domain.Events;
public class ThresholdAlertEvent : TelemetryEvent
{
    public string ParameterName { get; private set; }
    public double ParameterValue { get; private set; }
    public double ThresholdValue { get; private set; }
    public AlertSeverity Severity { get; private set; }
    public string AlertMessage { get; private set; }

    public ThresholdAlertEvent(Guid vehicleId, string parameterName, double parameterValue,
        double thresholdValue, AlertSeverity severity, string alertMessage)
        : base(vehicleId)
    {
        ParameterName = parameterName;
        ParameterValue = parameterValue;
        ThresholdValue = thresholdValue;
        Severity = severity;
        AlertMessage = alertMessage;
    }
}