using DriverService.Domain.Enums;
using DriverService.Domain.Models;

namespace DriverService.Domain.Repositories
{
    public interface IDriverRepository
    {
        Task<Driver> GetByIdAsync(Guid id);
        Task<IEnumerable<Driver>> ListAsync(int pageSize, int pageNumber, string filter = null, DriverStatus? status = null);
        Task<int> CountAsync(string filter = null, DriverStatus? status = null);
        Task<Driver> AddAsync(Driver driver);
        Task<Driver> UpdateAsync(Driver driver);
        Task<bool> DeleteAsync(Guid id);
        Task<Assignment> GetAssignmentByIdAsync(Guid assignmentId);
        Task<IEnumerable<Assignment>> GetDriverAssignmentsAsync(Guid driverId, DateTime fromDate, DateTime toDate);
        Task<IEnumerable<ScheduleEntry>> GetDriverScheduleAsync(Guid driverId, DateTime fromDate, DateTime toDate);
    }
}
