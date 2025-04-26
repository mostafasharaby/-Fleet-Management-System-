using TelemetryService.Domain.Models;

namespace TelemetryService.Domain.Repositories
{
    public interface ITelemetryRepository
    {
        Task<TelemetryData> GetByIdAsync(Guid id);
        Task<IEnumerable<TelemetryData>> GetByVehicleIdAsync(Guid vehicleId, int limit = 100);
        Task<IEnumerable<TelemetryData>> GetByVehicleIdTimeRangeAsync(Guid vehicleId, DateTime startTime, DateTime endTime);
        Task AddAsync(TelemetryData telemetryData);
        Task AddRangeAsync(IEnumerable<TelemetryData> telemetryData);
        Task<IEnumerable<TelemetryData>> GetLatestForAllVehiclesAsync();
    }
}
