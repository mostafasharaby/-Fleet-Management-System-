using FleetManagement.Client.Services;
using MaintenanceService.API.Protos;

namespace FleetManagement.Client.Workers
{
    internal class MaintenanceWorker : BackgroundService
    {
        private readonly ILogger<MaintenanceWorker> _logger;
        private readonly MaintenanceServiceClient _maintenanceServiceClient;

        public MaintenanceWorker(
            MaintenanceServiceClient maintenanceServiceClient,
            ILogger<MaintenanceWorker> logger)
        {
            _maintenanceServiceClient = maintenanceServiceClient;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!stoppingToken.IsCancellationRequested)
            {
                //await GetMaintenanceSchedule();
                //await RecordMaintenanceEvent();
                //await ScheduleMaintenance();
                //await UpdateMaintenanceStatus();
                //await GetMaintenanceHistory();
                //await GetVehicleHealthMetrics();
            }
        }

        private async Task GetMaintenanceSchedule()
        {
            Guid vehicleId = Guid.Parse("8DB05DA5-9AE5-46D7-BC3C-10F260EAB20C");
            bool includeCompleted = true;

            try
            {
                _logger.LogInformation($"Fetching maintenance schedule for vehicle ID: {vehicleId}");
                var tasks = await _maintenanceServiceClient.GetMaintenanceScheduleAsync(vehicleId, includeCompleted);

                if (tasks.Count > 0)
                {
                    Console.WriteLine($"Found {tasks.Count} maintenance tasks for vehicle {vehicleId}:");
                    foreach (var task in tasks)
                    {
                        Console.WriteLine($"Task ID: {task.TaskId}");
                        Console.WriteLine($"Description: {task.TaskDescription}");
                        Console.WriteLine($"Scheduled Date: {DateTimeOffset.FromUnixTimeSeconds((long)task.ScheduledDate).DateTime}");
                        Console.WriteLine($"Status: {task.Status}");
                        Console.WriteLine($"Required Parts: {string.Join(", ", task.RequiredParts.Select(p => $"{p.PartName} (Qty: {p.Quantity})"))}");
                        Console.WriteLine("---");
                    }
                }
                else
                {
                    Console.WriteLine($"No maintenance tasks found for vehicle {vehicleId}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving maintenance schedule for vehicle {vehicleId}");
                Console.WriteLine($"Error retrieving maintenance schedule: {ex.Message}");
            }
        }

        private async Task RecordMaintenanceEvent()
        {
            string vehicleId = "8DB05DA5-9AE5-46D7-BC3C-10F260EAB20C";
            var maintenanceEvent = new MaintenanceEvent
            {
                VehicleId = vehicleId,
                Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                Description = "Oil change and tire rotation",
                OdometerReading = 15000,
                PerformedBy = "Mechanic John",
                Notes = "Completed as per schedule"
            };

            try
            {
                _logger.LogInformation($"Recording maintenance event for vehicle ID: {vehicleId}");
                var (success, eventId, message) = await _maintenanceServiceClient.RecordMaintenanceEventAsync(maintenanceEvent);

                if (success)
                {
                    Console.WriteLine($"Maintenance Event Recorded: ID={eventId}");
                    Console.WriteLine($"Message: {message}");
                }
                else
                {
                    Console.WriteLine($"Failed to record maintenance event: {message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error recording maintenance event for vehicle {vehicleId}");
                Console.WriteLine($"Error recording maintenance event: {ex.Message}");
            }
        }

        private async Task ScheduleMaintenance()
        {
            string vehicleId = "8DB05DA5-9AE5-46D7-BC3C-10F260EAB20C";
            var task = new MaintenanceTask
            {
                VehicleId = vehicleId,
                TaskDescription = "Brake inspection",
                ScheduledDate = new DateTimeOffset(DateTime.UtcNow.AddDays(7)).ToUnixTimeSeconds(),
                Status = MaintenanceStatus.Scheduled,
            };
            task.RequiredParts.Add(new RequiredPart { PartId = "1", PartName = "Brake Pads", Quantity = 4 });

            try
            {
                _logger.LogInformation($"Scheduling maintenance task: {task.TaskDescription}");
                var (success, taskId, message) = await _maintenanceServiceClient.ScheduleMaintenanceAsync(task);

                if (success)
                {
                    Console.WriteLine($"Maintenance Task Scheduled: ID={taskId}");
                    Console.WriteLine($"Message: {message}");
                }
                else
                {
                    Console.WriteLine($"Failed to schedule maintenance task: {message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error scheduling maintenance task: {task.TaskDescription}");
                Console.WriteLine($"Error scheduling maintenance task: {ex.Message}");
            }
        }

        private async Task UpdateMaintenanceStatus()
        {
            Guid taskId = Guid.NewGuid(); // Replace with a valid task ID
            MaintenanceStatus newStatus = MaintenanceStatus.Completed;
            string updatedBy = "Mechanic Jane";
            string notes = "Task completed successfully";

            try
            {
                _logger.LogInformation($"Updating maintenance status for task ID: {taskId}");
                var (success, message) = await _maintenanceServiceClient.UpdateMaintenanceStatusAsync(taskId, newStatus, updatedBy, notes);

                if (success)
                {
                    Console.WriteLine($"Maintenance Status Updated: {message}");
                }
                else
                {
                    Console.WriteLine($"Failed to update maintenance status: {message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating maintenance status for task {taskId}");
                Console.WriteLine($"Error updating maintenance status: {ex.Message}");
            }
        }

        private async Task GetMaintenanceHistory()
        {
            Guid vehicleId = Guid.Parse("8DB05DA5-9AE5-46D7-BC3C-10F260EAB20C");
            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            DateTime endDate = DateTime.UtcNow;

            try
            {
                _logger.LogInformation($"Fetching maintenance history for vehicle ID: {vehicleId}");
                var (events, totalCount, totalPages) = await _maintenanceServiceClient.GetMaintenanceHistoryAsync(
                    vehicleId, startDate, endDate, pageSize: 10, pageNumber: 1);

                Console.WriteLine($"Total Events: {totalCount}");
                Console.WriteLine($"Total Pages: {totalPages}");

                if (events.Count > 0)
                {
                    Console.WriteLine($"Found {events.Count} maintenance events for vehicle {vehicleId}:");
                    foreach (var evt in events)
                    {
                        Console.WriteLine($"Event ID: {evt.EventId}");
                        Console.WriteLine($"Description: {evt.Description}");
                        Console.WriteLine($"Event Date: {DateTimeOffset.FromUnixTimeSeconds(evt.Timestamp).DateTime}");
                        Console.WriteLine($"Odometer: {evt.OdometerReading}");
                        Console.WriteLine($"Performed By: {evt.PerformedBy}");
                        Console.WriteLine("---");
                    }
                }
                else
                {
                    Console.WriteLine($"No maintenance events found for vehicle {vehicleId}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving maintenance history for vehicle {vehicleId}");
                Console.WriteLine($"Error retrieving maintenance history: {ex.Message}");
            }
        }

        private async Task GetVehicleHealthMetrics()
        {
            Guid vehicleId = Guid.Parse("8DB05DA5-9AE5-46D7-BC3C-10F260EAB20C");
            try
            {
                _logger.LogInformation($"Fetching vehicle health metrics for vehicle ID: {vehicleId}");
                var metrics = await _maintenanceServiceClient.GetVehicleHealthMetricsAsync(vehicleId);

                Console.WriteLine($"Vehicle Health Metrics for Vehicle ID: {vehicleId}");
                Console.WriteLine($"Health Score: {metrics.OverallHealthScore}");
                Console.WriteLine($"Last Maintenance: {DateTimeOffset.FromUnixTimeSeconds(metrics.LastMaintenanceDate).DateTime}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving vehicle health metrics for vehicle {vehicleId}");
                Console.WriteLine($"Error retrieving vehicle health metrics: {ex.Message}");
            }
        }
    }
}