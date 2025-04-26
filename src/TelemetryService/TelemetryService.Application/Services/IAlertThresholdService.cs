using TelemetryService.Application.DTOs;

namespace TelemetryService.Application.Services
{
    public interface IAlertThresholdService
    {
        Task<IEnumerable<AlertThresholdDto>> GetAlertThresholdsByVehicleIdAsync(Guid vehicleId);
        Task<AlertThresholdDto> GetAlertThresholdByIdAsync(Guid id);
        Task<AlertThresholdDto> CreateAlertThresholdAsync(AlertThresholdDto alertThresholdDto);
        Task UpdateAlertThresholdAsync(AlertThresholdDto alertThresholdDto);
        Task DeleteAlertThresholdAsync(Guid id);
    }
}
