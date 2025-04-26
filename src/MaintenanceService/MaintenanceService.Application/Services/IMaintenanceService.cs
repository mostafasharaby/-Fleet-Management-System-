using MaintenanceService.Domain.Enums;
using MaintenanceService.Domain.Models;

namespace MaintenanceService.Application.Services
{
    public interface IMaintenanceService
    {
        Task<MaintenanceTask> GetMaintenanceTaskAsync(Guid taskId);
        Task<IEnumerable<MaintenanceTask>> GetScheduledTasksAsync(Guid vehicleId, bool includeCompleted);
        Task<MaintenanceTask> ScheduleMaintenanceAsync(MaintenanceTask task);
        Task UpdateMaintenanceStatusAsync(Guid taskId, MaintenanceStatus newStatus, string updatedBy, string notes);
        Task<MaintenanceEvent> RecordMaintenanceEventAsync(MaintenanceEvent maintenanceEvent);
        Task<(IEnumerable<MaintenanceEvent> Events, int TotalCount, int TotalPages)> GetMaintenanceHistoryAsync(
            Guid vehicleId, DateTime startDate, DateTime endDate, int pageSize, int pageNumber);
        Task<VehicleHealthMetrics> GetVehicleHealthMetricsAsync(Guid vehicleId);
    }
}
