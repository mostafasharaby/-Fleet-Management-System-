using DriverService.Domain.Enums;
using DriverService.Domain.Models;
using DriverService.Domain.Repositories;
using DriverService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DriverService.Infrastructure.Repositories
{
    public class DriverRepository : IDriverRepository
    {
        private readonly DriverDbContext _context;

        public DriverRepository(DriverDbContext context)
        {
            _context = context;
        }

        public async Task<Driver> GetByIdAsync(Guid id)
        {
            return await _context.Drivers
                .Include(d => d.Assignments)
                .Include(d => d.Schedule)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<Driver>> ListAsync(int pageSize, int pageNumber, string filter = null, DriverStatus? status = null)
        {
            var query = _context.Drivers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(d =>
                    d.FirstName.Contains(filter) ||
                    d.LastName.Contains(filter) ||
                    d.Email.Contains(filter) ||
                    d.LicenseNumber.Contains(filter));
            }

            if (status.HasValue)
            {
                query = query.Where(d => d.Status == status.Value);
            }

            return await query
                .OrderBy(d => d.LastName)
                .ThenBy(d => d.FirstName)
                .ToListAsync();
        }

        public async Task<int> CountAsync(string filter = null, DriverStatus? status = null)
        {
            var query = _context.Drivers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(d =>
                    d.FirstName.Contains(filter) ||
                    d.LastName.Contains(filter) ||
                    d.Email.Contains(filter) ||
                    d.LicenseNumber.Contains(filter));
            }

            if (status.HasValue)
            {
                query = query.Where(d => d.Status == status.Value);
            }

            return await query.CountAsync();
        }

        public async Task<Driver> AddAsync(Driver driver)
        {
            await _context.Drivers.AddAsync(driver);
            await _context.SaveChangesAsync();
            return driver;
        }

        public async Task<Driver> UpdateAsync(Driver driver)
        {
            _context.Drivers.Update(driver);
            await _context.SaveChangesAsync();
            return driver;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var driver = await _context.Drivers.FindAsync(id);
            if (driver == null)
                return false;

            _context.Drivers.Remove(driver);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Assignment> GetAssignmentByIdAsync(Guid assignmentId)
        {
            return await _context.Assignments.FindAsync(assignmentId);
        }

        public async Task<IEnumerable<Assignment>> GetDriverAssignmentsAsync(Guid driverId, DateTime fromDate, DateTime toDate)
        {
            return await _context.Assignments
                .Where(a => a.DriverId == driverId && a.EndTime >= fromDate && a.StartTime <= toDate)
                .OrderBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<ScheduleEntry>> GetDriverScheduleAsync(Guid driverId, DateTime fromDate, DateTime toDate)
        {
            return await _context.ScheduleEntries
                .Where(s => s.DriverId == driverId && s.EndTime >= fromDate && s.StartTime <= toDate)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }
    }
}
