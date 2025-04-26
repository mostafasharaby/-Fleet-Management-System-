using MaintenanceService.Domain.Models;

namespace MaintenanceService.Domain.Repositories
{
    public interface IMaintenanceRepository
    {
        Task<MaintenanceTask> GetTaskByIdAsync(Guid id);
        Task<IEnumerable<MaintenanceTask>> GetScheduledTasksForVehicleAsync(Guid vehicleId, bool includeCompleted);
        Task<MaintenanceTask> AddTaskAsync(MaintenanceTask task);
        Task UpdateTaskAsync(MaintenanceTask task);
        Task<IEnumerable<MaintenanceEvent>> GetMaintenanceHistoryAsync(Guid vehicleId, DateTime startDate, DateTime endDate, int pageSize, int pageNumber);
        Task<int> GetMaintenanceHistoryCountAsync(Guid vehicleId, DateTime startDate, DateTime endDate);
        Task<MaintenanceEvent> RecordMaintenanceEventAsync(MaintenanceEvent maintenanceEvent);
        Task<VehicleHealthMetrics> GetVehicleHealthMetricsAsync(Guid vehicleId);
        Task<DateTime?> GetLastMaintenanceDateAsync(Guid vehicleId);
        Task<IEnumerable<MaintenanceTask>> GetOverdueMaintenanceTasksAsync();
    }
}
