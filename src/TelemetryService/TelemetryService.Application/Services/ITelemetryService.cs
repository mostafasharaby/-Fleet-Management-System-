using TelemetryService.Application.DTOs;

namespace TelemetryService.Application.Services
{
    public interface ITelemetryService
    {
        Task<TelemetryDataDto> GetTelemetryDataByIdAsync(Guid id);
        Task<IEnumerable<TelemetryDataDto>> GetTelemetryDataByVehicleIdAsync(Guid vehicleId, int limit = 100);
        Task<IEnumerable<TelemetryDataDto>> GetTelemetryDataByTimeRangeAsync(Guid vehicleId, DateTime startTime, DateTime endTime);
        Task ProcessTelemetryDataAsync(TelemetryDataDto telemetryData);
        Task ProcessBatchTelemetryDataAsync(IEnumerable<TelemetryDataDto> telemetryDataBatch);
        Task<IEnumerable<TelemetryDataDto>> GetLatestTelemetryForAllVehiclesAsync();
    }
}
