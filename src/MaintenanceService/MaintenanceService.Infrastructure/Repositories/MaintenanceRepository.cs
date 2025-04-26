using MaintenanceService.Domain.Enums;
using MaintenanceService.Domain.Models;
using MaintenanceService.Domain.Repositories;
using MaintenanceService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MaintenanceService.Infrastructure.Repositories
{
    public class MaintenanceRepository : IMaintenanceRepository
    {
        private readonly MaintenanceDbContext _context;
        private readonly ILogger<MaintenanceRepository> _logger;

        public MaintenanceRepository(
            MaintenanceDbContext context,
            ILogger<MaintenanceRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<MaintenanceTask> GetTaskByIdAsync(Guid id)
        {
            return await _context.MaintenanceTasks
                .Include(t => t.RequiredParts)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<MaintenanceTask>> GetScheduledTasksForVehicleAsync(Guid vehicleId, bool includeCompleted)
        {
            var query = _context.MaintenanceTasks
                .Include(t => t.RequiredParts)
                .Where(t => t.VehicleId == vehicleId);

            if (!includeCompleted)
            {
                query = query.Where(t => t.Status != MaintenanceStatus.Completed &&
                                        t.Status != MaintenanceStatus.Canceled);
            }

            return await query.OrderBy(t => t.ScheduledDate).ToListAsync();
        }

        public async Task<MaintenanceTask> AddTaskAsync(MaintenanceTask task)
        {
            await _context.MaintenanceTasks.AddAsync(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task UpdateTaskAsync(MaintenanceTask task)
        {
            _context.MaintenanceTasks.Update(task);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<MaintenanceEvent>> GetMaintenanceHistoryAsync(
            Guid vehicleId, DateTime startDate, DateTime endDate, int pageSize, int pageNumber)
        {
            return await _context.MaintenanceEvents
                .Include(e => e.PartsReplaced)
                .Where(e => e.VehicleId == vehicleId)
                .OrderByDescending(e => e.Timestamp)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetMaintenanceHistoryCountAsync(
            Guid vehicleId, DateTime startDate, DateTime endDate)
        {
            return await _context.MaintenanceEvents
                .Where(e => e.VehicleId == vehicleId ||
                           e.Timestamp >= startDate ||
                           e.Timestamp <= endDate)
                .CountAsync();
        }

        public async Task<MaintenanceEvent> RecordMaintenanceEventAsync(MaintenanceEvent maintenanceEvent)
        {
            await _context.MaintenanceEvents.AddAsync(maintenanceEvent);
            await _context.SaveChangesAsync();
            return maintenanceEvent;
        }

        public async Task<VehicleHealthMetrics> GetVehicleHealthMetricsAsync(Guid vehicleId)
        {
            // In a real system, this would involve complex calculations based on
            // maintenance history, telemetry data, etc.

            // For demonstration purposes, we'll generate some placeholder data
            var lastMaintenance = await GetLastMaintenanceDateAsync(vehicleId);
            var nextMaintenance = await _context.MaintenanceTasks
                .Where(t => t.VehicleId == vehicleId &&
                           t.Status == MaintenanceStatus.Scheduled)
                .OrderBy(t => t.ScheduledDate)
                .Select(t => t.ScheduledDate)
                .FirstOrDefaultAsync();

            var maintenanceCount = await _context.MaintenanceEvents
                .Where(e => e.VehicleId == vehicleId)
                .CountAsync();

            // Sample component health metrics
            var componentHealths = new List<ComponentHealthMetric>
            {
                new ComponentHealthMetric
                {
                    ComponentName = "Engine",
                    HealthScore = 0.95,
                    Status = "Good",
                    LastChecked = lastMaintenance ?? DateTime.UtcNow.AddDays(-30)
                },
                new ComponentHealthMetric
                {
                    ComponentName = "Transmission",
                    HealthScore = 0.88,
                    Status = "Good",
                    LastChecked = lastMaintenance ?? DateTime.UtcNow.AddDays(-30)
                },
                new ComponentHealthMetric
                {
                    ComponentName = "Brakes",
                    HealthScore = 0.75,
                    Status = "Fair",
                    LastChecked = lastMaintenance ?? DateTime.UtcNow.AddDays(-30)
                },
                new ComponentHealthMetric
                {
                    ComponentName = "Tires",
                    HealthScore = 0.82,
                    Status = "Good",
                    LastChecked = lastMaintenance ?? DateTime.UtcNow.AddDays(-30)
                }
            };

            return new VehicleHealthMetrics
            {
                VehicleId = vehicleId,
                OverallHealthScore = componentHealths.Average(ch => ch.HealthScore),
                LastMaintenanceDate = lastMaintenance,
                NextMaintenanceDue = nextMaintenance,
                MaintenanceEventsCount = maintenanceCount,
                ComponentHealths = componentHealths
            };
        }

        public async Task<DateTime?> GetLastMaintenanceDateAsync(Guid vehicleId)
        {
            return await _context.MaintenanceEvents
                .Where(e => e.VehicleId == vehicleId)
                .OrderByDescending(e => e.Timestamp)
                .Select(e => e.Timestamp)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<MaintenanceTask>> GetOverdueMaintenanceTasksAsync()
        {
            var today = DateTime.UtcNow.Date;
            return await _context.MaintenanceTasks
                .Include(t => t.RequiredParts)
                .Where(t => t.Status == MaintenanceStatus.Scheduled &&
                           t.ScheduledDate.Date < today)
                .OrderBy(t => t.ScheduledDate)
                .ToListAsync();
        }
    }
}