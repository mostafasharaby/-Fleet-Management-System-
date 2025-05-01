
using DriverService.API.Protos;
using FleetManagement.Client.Services;

namespace FleetManagement.Client.Workers
{
    internal class DriverWorker : BackgroundService
    {
        private readonly ILogger<DriverWorker> _logger;
        private readonly DriverServiceClient _driverServiceClient;

        public DriverWorker(
            DriverServiceClient driverServiceClient,
            ILogger<DriverWorker> logger)
        {
            _driverServiceClient = driverServiceClient;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!stoppingToken.IsCancellationRequested)
            {
                // Uncomment methods to test specific operations
                await GetDriverById();
                await ListDrivers();
                await CreateDriver();
                await UpdateDriver();
                await DeleteDriver();
                await GetDriverAssignments();
                await AssignVehicle();
                await CompleteAssignment();
                await GetDriverAvailability();
                await ScheduleDriver();
            }
        }

        private async Task GetDriverById()
        {
            Guid driverId = Guid.NewGuid(); // Replace with a valid driver ID
            try
            {
                _logger.LogInformation($"Fetching driver with ID: {driverId}");
                var driver = await _driverServiceClient.GetDriverAsync(driverId);

                if (driver != null)
                {
                    Console.WriteLine($"Driver Found: ID={driver.Id}");
                    Console.WriteLine($"Name: {driver.FirstName} {driver.LastName}");
                    Console.WriteLine($"Email: {driver.Email}");
                    Console.WriteLine($"License Number: {driver.LicenseNumber}");
                }
                else
                {
                    Console.WriteLine($"Driver with ID {driverId} not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving driver with ID {driverId}");
                Console.WriteLine($"Error retrieving driver: {ex.Message}");
            }
        }

        private async Task ListDrivers()
        {
            try
            {
                _logger.LogInformation("Fetching list of drivers");
                var (drivers, totalCount, pageCount) = await _driverServiceClient.ListDriversAsync(pageSize: 10, pageNumber: 1);

                Console.WriteLine($"Total Drivers: {totalCount}");
                Console.WriteLine($"Page Count: {pageCount}");

                foreach (var driver in drivers)
                {
                    Console.WriteLine($"Driver Found: ID={driver.Id}");
                    Console.WriteLine($"Name: {driver.FirstName} {driver.LastName}");
                    Console.WriteLine($"Email: {driver.Email}");
                    Console.WriteLine($"License Number: {driver.LicenseNumber}");
                    Console.WriteLine("---");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing drivers");
                Console.WriteLine($"Error listing drivers: {ex.Message}");
            }
        }

        private async Task CreateDriver()
        {
            try
            {
                _logger.LogInformation("Creating a new driver");
                var driver = await _driverServiceClient.CreateDriverAsync(
                    firstName: "John",
                    lastName: "Doe",
                    email: "john.doe@example.com",
                    phoneNumber: "123-456-7890",
                    licenseNumber: "D1234567",
                    licenseState: "CA",
                    licenseExpiry: DateTime.UtcNow.AddYears(2),
                    type: DriverType.FullTime
                );

                Console.WriteLine($"Driver Created: ID={driver.Id}");
                Console.WriteLine($"Name: {driver.FirstName} {driver.LastName}");
                Console.WriteLine($"Email: {driver.Email}");
                Console.WriteLine($"License Number: {driver.LicenseNumber}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating driver");
                Console.WriteLine($"Error creating driver: {ex.Message}");
            }
        }

        private async Task UpdateDriver()
        {
            Guid driverId = Guid.NewGuid(); // Replace with a valid driver ID
            try
            {
                _logger.LogInformation($"Updating driver with ID: {driverId}");
                var driver = await _driverServiceClient.UpdateDriverAsync(
                    driverId: driverId,
                    firstName: "Jane",
                    lastName: "Doe",
                    email: "jane.doe@example.com",
                    phoneNumber: "987-654-3210",
                    licenseNumber: "D7654321",
                    licenseState: "NY",
                    licenseExpiry: DateTime.UtcNow.AddYears(3),
                    status: DriverStatus.Active,
                    type: DriverType.PartTime
                );

                if (driver != null)
                {
                    Console.WriteLine($"Driver Updated: ID={driver.Id}");
                    Console.WriteLine($"Name: {driver.FirstName} {driver.LastName}");
                    Console.WriteLine($"Email: {driver.Email}");
                    Console.WriteLine($"License Number: {driver.LicenseNumber}");
                }
                else
                {
                    Console.WriteLine($"Driver with ID {driverId} not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating driver with ID {driverId}");
                Console.WriteLine($"Error updating driver: {ex.Message}");
            }
        }

        private async Task DeleteDriver()
        {
            Guid driverId = Guid.NewGuid(); // Replace with a valid driver ID
            try
            {
                _logger.LogInformation($"Deleting driver with ID: {driverId}");
                var (success, message) = await _driverServiceClient.DeleteDriverAsync(driverId);

                if (success)
                {
                    Console.WriteLine($"Driver deleted successfully: {message}");
                }
                else
                {
                    Console.WriteLine($"Failed to delete driver: {message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting driver with ID {driverId}");
                Console.WriteLine($"Error deleting driver: {ex.Message}");
            }
        }

        private async Task GetDriverAssignments()
        {
            Guid driverId = Guid.NewGuid(); // Replace with a valid driver ID
            DateTime fromDate = DateTime.UtcNow.AddDays(-7);
            DateTime toDate = DateTime.UtcNow.AddDays(7);

            try
            {
                _logger.LogInformation($"Fetching assignments for driver {driverId} from {fromDate} to {toDate}");
                var assignments = await _driverServiceClient.GetDriverAssignmentsAsync(driverId, fromDate, toDate);

                if (assignments.Count > 0)
                {

                    Console.WriteLine($"Found {assignments.Count} assignments for driver {driverId}:");
                    foreach (var assignment in assignments)
                    {
                        Console.WriteLine($"Assignment ID: {assignment.Id}");
                        Console.WriteLine($"Vehicle ID: {assignment.VehicleId}");
                        Console.WriteLine($"Start Time: {DateTimeOffset.FromUnixTimeSeconds(assignment.StartTime).UtcDateTime}");
                        Console.WriteLine($"End Time: {DateTimeOffset.FromUnixTimeSeconds(assignment.EndTime).UtcDateTime}");
                        Console.WriteLine($"Notes: {assignment.Notes}");
                        Console.WriteLine("---");
                    }
                }
                else
                {
                    Console.WriteLine($"No assignments found for driver {driverId}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving assignments for driver {driverId}");
                Console.WriteLine($"Error retrieving assignments: {ex.Message}");
            }
        }

        private async Task AssignVehicle()
        {
            Guid driverId = Guid.NewGuid(); // Replace with a valid driver ID
            Guid vehicleId = Guid.Parse("8DB05DA5-9AE5-46D7-BC3C-10F260EAB20C"); // Replace with a valid vehicle ID
            DateTime startTime = DateTime.UtcNow;
            DateTime endTime = DateTime.UtcNow.AddDays(1);
            string notes = "Assigned for delivery";

            try
            {
                _logger.LogInformation($"Assigning vehicle {vehicleId} to driver {driverId}");
                var assignment = await _driverServiceClient.AssignVehicleAsync(driverId, vehicleId, startTime, endTime, notes);

                if (assignment != null)
                {
                    Console.WriteLine($"Vehicle Assigned: Assignment ID={assignment.Id}");
                    Console.WriteLine($"Driver ID: {assignment.DriverId}");
                    Console.WriteLine($"Vehicle ID: {assignment.VehicleId}");
                    Console.WriteLine($"Start Time: {DateTimeOffset.FromUnixTimeSeconds(assignment.StartTime).UtcDateTime}");
                    Console.WriteLine($"End Time: {DateTimeOffset.FromUnixTimeSeconds(assignment.EndTime).UtcDateTime}");
                    Console.WriteLine($"Notes: {assignment.Notes}");
                }
                else
                {
                    Console.WriteLine($"Failed to assign vehicle to driver {driverId}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error assigning vehicle {vehicleId} to driver {driverId}");
                Console.WriteLine($"Error assigning vehicle: {ex.Message}");
            }
        }

        private async Task CompleteAssignment()
        {
            Guid assignmentId = Guid.NewGuid(); // Replace with a valid assignment ID
            float finalOdometer = 15000.0f;
            float fuelLevel = 50.0f;
            string notes = "Assignment completed successfully";

            try
            {
                _logger.LogInformation($"Completing assignment {assignmentId}");
                var assignment = await _driverServiceClient.CompleteAssignmentAsync(assignmentId, finalOdometer, fuelLevel, notes);

                if (assignment != null)
                {
                    Console.WriteLine($"Assignment Completed: ID={assignment.Id}");
                    Console.WriteLine($"Final Odometer: {assignment.EndOdometer}");
                    //Console.WriteLine($"Fuel Level: {assignment.l}");
                    Console.WriteLine($"Notes: {assignment.Notes}");
                }
                else
                {
                    Console.WriteLine($"Assignment with ID {assignmentId} not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error completing assignment {assignmentId}");
                Console.WriteLine($"Error completing assignment: {ex.Message}");
            }
        }

        private async Task GetDriverAvailability()
        {
            Guid driverId = Guid.NewGuid();
            DateTime fromDate = DateTime.UtcNow.AddDays(-7);
            DateTime toDate = DateTime.UtcNow.AddDays(7);

            try
            {
                _logger.LogInformation($"Fetching availability for driver {driverId} from {fromDate} to {toDate}");
                var availabilitySlots = await _driverServiceClient.GetDriverAvailabilityAsync(driverId, fromDate, toDate);

                if (availabilitySlots.Count > 0)
                {
                    Console.WriteLine($"Found {availabilitySlots.Count} availability slots for driver {driverId}:");
                    foreach (var slot in availabilitySlots)
                    {
                        Console.WriteLine($"Start Time: {DateTimeOffset.FromUnixTimeSeconds(slot.StartTime).UtcDateTime}");
                        Console.WriteLine($"End Time: {DateTimeOffset.FromUnixTimeSeconds(slot.EndTime).UtcDateTime}");
                        Console.WriteLine($"Is Available: {slot.IsAvailable}");
                        Console.WriteLine($"Assignment ID: {slot.AssignmentId}");
                        Console.WriteLine("---");
                    }
                }
                else
                {
                    Console.WriteLine($"No availability slots found for driver {driverId}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving availability for driver {driverId}");
                Console.WriteLine($"Error retrieving availability: {ex.Message}");
            }
        }

        private async Task ScheduleDriver()
        {
            Guid driverId = Guid.NewGuid(); // Replace with a valid driver ID
            var scheduleSlots = new List<(DateTime StartTime, DateTime EndTime, ScheduleType Type, string Notes)>
            {
                (DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(8), ScheduleType.Work, "Morning shift"),
                (DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(2).AddHours(8), ScheduleType.Break, "Day off")
            };

            try
            {
                _logger.LogInformation($"Scheduling driver {driverId} with {scheduleSlots.Count} slots");
                var (success, message, conflicts) = await _driverServiceClient.ScheduleDriverAsync(driverId, scheduleSlots);

                Console.WriteLine($"Scheduling Result: {message}");
                if (success)
                {
                    Console.WriteLine("Driver scheduled successfully.");
                }
                else
                {
                    Console.WriteLine($"Scheduling completed with {conflicts.Count} conflicts:");
                    foreach (var conflict in conflicts)
                    {
                        Console.WriteLine($"Conflict: Start={DateTimeOffset.FromUnixTimeSeconds(conflict.StartTime).UtcDateTime}," +
                            $" End={DateTimeOffset.FromUnixTimeSeconds(conflict.EndTime).UtcDateTime}, Reason={conflict.Reason}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error scheduling driver {driverId}");
                Console.WriteLine($"Error scheduling driver: {ex.Message}");
            }
        }
    }
}
