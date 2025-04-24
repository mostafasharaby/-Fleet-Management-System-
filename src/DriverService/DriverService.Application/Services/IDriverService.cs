using DriverService.Application.Models;
using DriverService.Domain.Enums;
using DriverService.Domain.Models;

namespace DriverService.Application.Services
{
    public interface IDriverService
    {
        Task<Driver> GetDriverAsync(Guid id);
        Task<(IEnumerable<Driver> Drivers, int TotalCount, int PageCount)> ListDriversAsync(int pageSize, int pageNumber, string filter = null, DriverStatus? status = null);
        Task<Driver> CreateDriverAsync(string firstName, string lastName, string email, string phoneNumber, string licenseNumber, string licenseState, DateTime licenseExpiry, DriverType type);
        Task<Driver> UpdateDriverAsync(Guid id, string firstName, string lastName, string email, string phoneNumber, string licenseNumber, string licenseState, DateTime licenseExpiry, DriverStatus status, DriverType type);
        Task<bool> DeleteDriverAsync(Guid id);
        Task<IEnumerable<Assignment>> GetDriverAssignmentsAsync(Guid driverId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<Assignment> AssignVehicleToDriverAsync(Guid driverId, Guid vehicleId, DateTime startTime, DateTime endTime, string notes);
        Task<Assignment> CompleteAssignmentAsync(Guid assignmentId, float finalOdometer, float fuelLevel, string notes);
        Task<List<(DateTime Start, DateTime End, bool IsAvailable, Guid? AssignmentId)>> GetDriverAvailabilityAsync(Guid driverId, DateTime fromDate, DateTime toDate);
        Task<(List<ScheduleEntry> ScheduleEntries, List<ScheduleConflict> Conflicts)> ScheduleDriverAsync(Guid driverId, List<ScheduleEntry> scheduleEntries);
        Task<bool> UpdateDriverStatusAsync(Guid driverId, DriverStatus status);
        Task<IEnumerable<ScheduleEntry>> GetDriverScheduleAsync(Guid driverId, DateTime? fromDate = null, DateTime? toDate = null);
    }
}
