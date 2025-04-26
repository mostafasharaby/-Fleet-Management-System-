using TelemetryService.Domain.Models;

namespace TelemetryService.Infrastructure.Caching
{
    public interface ITelemetryCache
    {
        Task<TelemetryData> GetLatestTelemetryDataAsync(Guid vehicleId);
        Task CacheTelemetryDataAsync(TelemetryData telemetryData);
        Task CacheAlertThresholdsAsync(Guid vehicleId, IEnumerable<AlertThreshold> thresholds);
        Task<IEnumerable<AlertThreshold>> GetAlertThresholdsAsync(Guid vehicleId);
        Task RemoveAlertThresholdAsync(Guid thresholdId, Guid vehicleId);
    }
}
