using DriverService.Application.Models;
using DriverService.Domain.Enums;
using DriverService.Domain.Models;
using DriverService.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace DriverService.Application.Services
{
    public class DriverService : IDriverService
    {
        private readonly IDriverRepository _driverRepository;
        private readonly ILogger<DriverService> _logger;

        public DriverService(IDriverRepository driverRepository, ILogger<DriverService> logger)
        {
            _driverRepository = driverRepository;
            _logger = logger;
        }

        public async Task<Driver> GetDriverAsync(Guid id)
        {
            return await _driverRepository.GetByIdAsync(id);
        }

        public async Task<(IEnumerable<Driver> Drivers, int TotalCount, int PageCount)> ListDriversAsync(
            int pageSize, int pageNumber, string filter = null, DriverStatus? status = null)
        {
            var drivers = await _driverRepository.ListAsync(pageSize, pageNumber, filter, status);
            var count = await _driverRepository.CountAsync(filter, status);
            var pageCount = (int)Math.Ceiling((double)count / pageSize);

            return (drivers, count, pageCount);
        }

        public async Task<Driver> CreateDriverAsync(
            string firstName,
            string lastName,
            string email,
            string phoneNumber,
            string licenseNumber,
            string licenseState,
            DateTime licenseExpiry,
            DriverType type)
        {
            var driver = Driver.Create(
                firstName,
                lastName,
                email,
                phoneNumber,
                licenseNumber,
                licenseState,
                licenseExpiry,
                type);

            await _driverRepository.AddAsync(driver);
            _logger.LogInformation($"Created new driver with ID {driver.Id}");
            return driver;
        }

        public async Task<Driver> UpdateDriverAsync(
            Guid id,
            string firstName,
            string lastName,
            string email,
            string phoneNumber,
            string licenseNumber,
            string licenseState,
            DateTime licenseExpiry,
            DriverStatus status,
            DriverType type)
        {
            var driver = await _driverRepository.GetByIdAsync(id);
            if (driver == null)
            {
                _logger.LogWarning($"Failed to update driver: Driver with ID {id} not found");
                return null;
            }

            driver.Update(
                firstName,
                lastName,
                email,
                phoneNumber,
                licenseNumber,
                licenseState,
                licenseExpiry,
                status,
                type);

            await _driverRepository.UpdateAsync(driver);
            _logger.LogInformation($"Updated driver with ID {driver.Id}");
            return driver;
        }

        public async Task<bool> DeleteDriverAsync(Guid id)
        {
            var result = await _driverRepository.DeleteAsync(id);
            if (result)
            {
                _logger.LogInformation($"Deleted driver with ID {id}");
            }
            else
            {
                _logger.LogWarning($"Failed to delete driver: Driver with ID {id} not found");
            }
            return result;
        }

        public async Task<IEnumerable<Assignment>> GetDriverAssignmentsAsync(Guid driverId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var from = fromDate ?? DateTime.UtcNow.AddDays(-30);
            var to = toDate ?? DateTime.UtcNow.AddDays(30);

            return await _driverRepository.GetDriverAssignmentsAsync(driverId, from, to);
        }

        public async Task<Assignment> AssignVehicleToDriverAsync(
            Guid driverId,
            Guid vehicleId,
            DateTime startTime,
            DateTime endTime,
            string notes)
        {
            var driver = await _driverRepository.GetByIdAsync(driverId);
            if (driver == null)
            {
                _logger.LogWarning($"Failed to assign vehicle: Driver with ID {driverId} not found");
                return null;
            }

            var assignment = driver.AssignVehicle(vehicleId, startTime, endTime, notes);
            await _driverRepository.UpdateAsync(driver);
            _logger.LogInformation($"Assigned vehicle {vehicleId} to driver {driverId}");
            return assignment;
        }

        public async Task<Assignment> CompleteAssignmentAsync(
            Guid assignmentId,
            float finalOdometer,
            float fuelLevel,
            string notes)
        {
            var assignment = await _driverRepository.GetAssignmentByIdAsync(assignmentId);
            if (assignment == null)
            {
                _logger.LogWarning($"Failed to complete assignment: Assignment with ID {assignmentId} not found");
                return null;
            }

            var driver = await _driverRepository.GetByIdAsync(assignment.DriverId);
            if (driver == null)
            {
                _logger.LogWarning($"Failed to complete assignment: Driver with ID {assignment.DriverId} not found");
                return null;
            }

            var updatedAssignment = driver.CompleteAssignment(assignmentId, finalOdometer, fuelLevel, notes);
            await _driverRepository.UpdateAsync(driver);
            _logger.LogInformation($"Completed assignment {assignmentId} for driver {driver.Id}");
            return updatedAssignment;
        }
        public async Task<List<(DateTime Start, DateTime End, bool IsAvailable, Guid? AssignmentId)>> GetDriverAvailabilityAsync(
          Guid driverId,
          DateTime fromDate,
          DateTime toDate)
        {
            _logger.LogInformation($"Getting availability for driver {driverId} from {fromDate} to {toDate}");

            var driver = await _driverRepository.GetByIdAsync(driverId);
            if (driver == null)
            {
                _logger.LogWarning($"Failed to get availability: Driver with ID {driverId} not found");
                return new List<(DateTime, DateTime, bool, Guid?)>();
            }

            var availability = driver.GetAvailability(fromDate, toDate);
            _logger.LogInformation($"Retrieved {availability.Count} availability slots for driver {driverId}");
            return availability;
        }

        public async Task<(List<ScheduleEntry> ScheduleEntries, List<ScheduleConflict> Conflicts)> ScheduleDriverAsync(
            Guid driverId,
            List<ScheduleEntry> scheduleEntries)
        {
            _logger.LogInformation($"Scheduling driver {driverId} with {scheduleEntries.Count} entries");

            var driver = await _driverRepository.GetByIdAsync(driverId);
            if (driver == null)
            {
                _logger.LogWarning($"Failed to schedule driver: Driver with ID {driverId} not found");
                return (new List<ScheduleEntry>(), new List<ScheduleConflict>());
            }

            var conflicts = new List<ScheduleConflict>();
            var validEntries = new List<ScheduleEntry>();

            foreach (var entry in scheduleEntries)
            {
                // Clone the list to avoid modifying the original if there's a conflict
                var tempValidEntries = new List<ScheduleEntry>(validEntries);
                tempValidEntries.Add(entry);

                try
                {
                    driver.AddScheduleEntries(new List<ScheduleEntry> { entry });
                    validEntries.Add(entry);
                }
                catch (InvalidOperationException ex)
                {
                    conflicts.Add(new ScheduleConflict
                    {
                        StartTime = entry.StartTime,
                        EndTime = entry.EndTime,
                        Reason = ex.Message
                    });
                }
            }

            // Only update if there are valid entries to add
            if (validEntries.Count > 0)
            {
                await _driverRepository.UpdateAsync(driver);
                _logger.LogInformation($"Successfully scheduled {validEntries.Count} entries for driver {driverId}");
            }

            if (conflicts.Count > 0)
            {
                _logger.LogWarning($"Found {conflicts.Count} scheduling conflicts for driver {driverId}");
            }

            return (validEntries, conflicts);
        }

        public async Task<bool> UpdateDriverStatusAsync(Guid driverId, DriverStatus status)
        {
            _logger.LogInformation($"Updating status for driver {driverId} to {status}");

            var driver = await _driverRepository.GetByIdAsync(driverId);
            if (driver == null)
            {
                _logger.LogWarning($"Failed to update driver status: Driver with ID {driverId} not found");
                return false;
            }

            driver.SetStatus(status);
            await _driverRepository.UpdateAsync(driver);
            _logger.LogInformation($"Updated status for driver {driverId} to {status}");
            return true;
        }

        public async Task<IEnumerable<ScheduleEntry>> GetDriverScheduleAsync(Guid driverId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var from = fromDate ?? DateTime.UtcNow.AddDays(-30);
            var to = toDate ?? DateTime.UtcNow.AddDays(30);

            _logger.LogInformation($"Getting schedule for driver {driverId} from {from} to {to}");

            var driver = await _driverRepository.GetByIdAsync(driverId);
            if (driver == null)
            {
                _logger.LogWarning($"Failed to get schedule: Driver with ID {driverId} not found");
                return Enumerable.Empty<ScheduleEntry>();
            }

            return await _driverRepository.GetDriverScheduleAsync(driverId, from, to);
        }

    }
}
