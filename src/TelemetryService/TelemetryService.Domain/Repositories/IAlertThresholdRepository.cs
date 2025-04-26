using TelemetryService.Domain.Models;

namespace TelemetryService.Domain.Repositories
{
    public interface IAlertThresholdRepository
    {
        Task<IEnumerable<AlertThreshold>> GetByVehicleIdAsync(Guid vehicleId);
        Task<AlertThreshold> GetByIdAsync(Guid id);
        Task AddAsync(AlertThreshold alertThreshold);
        Task UpdateAsync(AlertThreshold alertThreshold);
        Task DeleteAsync(Guid id);
    }
}
