using MaintenanceService.Application.DTOs;
using MaintenanceService.Domain.Enums;
using MaintenanceService.Domain.Events;
using MaintenanceService.Domain.Models;
using MaintenanceService.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace MaintenanceService.Application.Services
{
    public class MaintenanceService : IMaintenanceService
    {
        private readonly IMaintenanceRepository _maintenanceRepository;
        private readonly ILogger<MaintenanceService> _logger;
        private readonly Dictionary<string, MaintenanceSubscription> _alertSubscriptions;

        public MaintenanceService(
            IMaintenanceRepository maintenanceRepository,
            ILogger<MaintenanceService> logger)
        {
            _maintenanceRepository = maintenanceRepository;
            _logger = logger;
            _alertSubscriptions = new Dictionary<string, MaintenanceSubscription>();
        }

        public async Task<MaintenanceTask> GetMaintenanceTaskAsync(Guid taskId)
        {
            _logger.LogInformation("Retrieving maintenance task with ID: {TaskId}", taskId);
            var task = await _maintenanceRepository.GetTaskByIdAsync(taskId);
            if (task == null)
            {
                _logger.LogWarning("Maintenance task with ID: {TaskId} not found", taskId);
                throw new KeyNotFoundException($"Maintenance task with ID {taskId} not found");
            }
            return task;
        }

        public async Task<IEnumerable<MaintenanceTask>> GetScheduledTasksAsync(Guid vehicleId, bool includeCompleted)
        {
            _logger.LogInformation("Retrieving scheduled tasks for vehicle: {VehicleId}, includeCompleted: {IncludeCompleted}", vehicleId, includeCompleted);
            return await _maintenanceRepository.GetScheduledTasksForVehicleAsync(vehicleId, includeCompleted);
        }

        public async Task<MaintenanceTask> ScheduleMaintenanceAsync(MaintenanceTask task)
        {
            _logger.LogInformation("Scheduling maintenance task for vehicle: {VehicleId}", task.VehicleId);
            var addedTask = await _maintenanceRepository.AddTaskAsync(task);

            // Publish domain event
            task.DomainEvents.Add(new MaintenanceScheduledEvent(
                addedTask.Id,
                addedTask.VehicleId,
                addedTask.ScheduledDate,
                addedTask.Type));

            return addedTask;
        }

        public async Task UpdateMaintenanceStatusAsync(Guid taskId, MaintenanceStatus newStatus, string updatedBy, string notes)
        {
            _logger.LogInformation("Updating maintenance status for task: {TaskId} to {NewStatus}", taskId, newStatus);
            var task = await GetMaintenanceTaskAsync(taskId);
            task.UpdateStatus(newStatus, updatedBy);
            await _maintenanceRepository.UpdateTaskAsync(task);
        }

        public async Task<MaintenanceEvent> RecordMaintenanceEventAsync(MaintenanceEvent maintenanceEvent)
        {
            _logger.LogInformation("Recording maintenance event for vehicle: {VehicleId}", maintenanceEvent.VehicleId);
            return await _maintenanceRepository.RecordMaintenanceEventAsync(maintenanceEvent);
        }

        public async Task<(IEnumerable<MaintenanceEvent> Events, int TotalCount, int TotalPages)> GetMaintenanceHistoryAsync(
            Guid vehicleId, DateTime startDate, DateTime endDate, int pageSize, int pageNumber)
        {
            _logger.LogInformation("Retrieving maintenance history for vehicle: {VehicleId}", vehicleId);
            var events = await _maintenanceRepository.GetMaintenanceHistoryAsync(vehicleId, startDate, endDate, pageSize, pageNumber);
            var totalCount = await _maintenanceRepository.GetMaintenanceHistoryCountAsync(vehicleId, startDate, endDate);
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            return (events, totalCount, totalPages);
        }

        public async Task<VehicleHealthMetrics> GetVehicleHealthMetricsAsync(Guid vehicleId)
        {
            _logger.LogInformation("Retrieving vehicle health metrics for vehicle: {VehicleId}", vehicleId);
            try
            {
                var healthMetrics = await _maintenanceRepository.GetVehicleHealthMetricsAsync(vehicleId);
                if (healthMetrics == null)
                {
                    _logger.LogWarning("No health metrics found for vehicle: {VehicleId}", vehicleId);
                    throw new InvalidOperationException($"No health metrics found for vehicle {vehicleId}");
                }
                _logger.LogInformation("Vehicle health metrics retrieved for vehicle: {VehicleId}, OverallHealthScore: {Score}",
                    vehicleId, healthMetrics.OverallHealthScore);
                return healthMetrics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving vehicle health metrics for vehicle: {VehicleId}", vehicleId);
                throw;
            }
        }


        public async Task<IEnumerable<MaintenanceTask>> GetOverdueMaintenanceTasksAsync()
        {
            _logger.LogInformation("Retrieving overdue maintenance tasks");
            return await _maintenanceRepository.GetOverdueMaintenanceTasksAsync();
        }

        public async Task<MaintenanceReportResult> GenerateMaintenanceReportAsync(
           string fleetId, DateTime startDate, DateTime endDate, string reportFormat)
        {
            try
            {
                // In a real implementation, this would generate a report based on the parameters
                // For now, we'll just simulate the process

                // Simulating report generation delay
                await Task.Delay(500);

                string reportUrl = $"https://reports.maintenanceservice.com/fleets/{fleetId}/{Guid.NewGuid()}.{reportFormat.ToLower()}";
                DateTime expiryTime = DateTime.UtcNow.AddDays(7);

                _logger.LogInformation("Generated maintenance report for fleet {FleetId} in {Format} format",
                    fleetId, reportFormat);

                return new MaintenanceReportResult
                {
                    Success = true,
                    ReportUrl = reportUrl,
                    ExpiryTime = expiryTime
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating maintenance report for fleet {FleetId}", fleetId);
                return new MaintenanceReportResult
                {
                    Success = false,
                    ReportUrl = "",
                    ExpiryTime = DateTime.MinValue
                };
            }
        }

        public Task<bool> SubscribeToMaintenanceAlertsAsync(
            string subscriberId, List<Guid> vehicleIds, List<MaintenanceAlertType> alertTypes)
        {
            try
            {
                lock (_alertSubscriptions)
                {
                    _alertSubscriptions[subscriberId] = new MaintenanceSubscription
                    {
                        SubscriberId = subscriberId,
                        VehicleIds = vehicleIds,
                        AlertTypes = alertTypes
                    };
                }

                _logger.LogInformation("Client {SubscriberId} subscribed to maintenance alerts", subscriberId);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error subscribing client {SubscriberId} to maintenance alerts", subscriberId);
                return Task.FromResult(false);
            }
        }

        public async Task PublishMaintenanceAlertAsync(MaintenanceAlertDto alert)
        {
            try
            {
                // In a real system, this would publish to a message bus or directly to subscribers
                // For now, we'll just log it
                _logger.LogInformation("Publishing maintenance alert: {AlertType} for vehicle {VehicleId}",
                    alert.AlertType, alert.VehicleId);

                // Simulate notification delay
                await Task.Delay(100);

                // In a real system with subscribers, you would iterate through subscribers and send the alert
                lock (_alertSubscriptions)
                {
                    foreach (var subscription in _alertSubscriptions.Values)
                    {
                        bool shouldSend = subscription.AlertTypes.Contains(alert.AlertType) &&
                                         (subscription.VehicleIds.Count == 0 || // Empty means all vehicles
                                          subscription.VehicleIds.Contains(alert.VehicleId));

                        if (shouldSend)
                        {
                            // In a real system, this would send to the subscriber
                            _logger.LogDebug("Sending alert to subscriber {SubscriberId}", subscription.SubscriberId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing maintenance alert for vehicle {VehicleId}", alert.VehicleId);
            }
        }

    }
}
