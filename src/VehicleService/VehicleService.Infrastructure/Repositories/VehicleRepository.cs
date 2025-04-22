using Microsoft.EntityFrameworkCore;
using VehicleService.Domain.Enums;
using VehicleService.Domain.Models;
using VehicleService.Domain.Repositories;
using VehicleService.Infrastructure.Data;

namespace VehicleService.Infrastructure.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly VehicleDbContext _context;

        public VehicleRepository(VehicleDbContext context)
        {
            _context = context;
        }

        public async Task<Vehicle> GetByIdAsync(Guid id)
        {
            return await _context.Vehicles
                .Include(v => v.MaintenanceHistory)
                .Include(v => v.FuelHistory)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<IEnumerable<Vehicle>> ListAsync(int pageSize, int pageNumber, string filter = null, VehicleStatus? status = null)
        {
            var query = _context.Vehicles.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(v =>
                    v.RegistrationNumber.Contains(filter) ||
                    v.Model.Contains(filter) ||
                    v.Manufacturer.Contains(filter) ||
                    v.VIN.Contains(filter));
            }

            if (status.HasValue)
            {
                query = query.Where(v => v.Status == status.Value);
            }

            return await query
                .OrderBy(v => v.RegistrationNumber)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountAsync(string filter = null, VehicleStatus? status = null)
        {
            var query = _context.Vehicles.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(v =>
                    v.RegistrationNumber.Contains(filter) ||
                    v.Model.Contains(filter) ||
                    v.Manufacturer.Contains(filter) ||
                    v.VIN.Contains(filter));
            }

            if (status.HasValue)
            {
                query = query.Where(v => v.Status == status.Value);
            }

            return await query.CountAsync();
        }

        public async Task<Vehicle> AddAsync(Vehicle vehicle)
        {
            await _context.Vehicles.AddAsync(vehicle);
            await _context.SaveChangesAsync();
            return vehicle;
        }

        public async Task<Vehicle> UpdateAsync(Vehicle vehicle)
        {
            _context.Vehicles.Update(vehicle);
            await _context.SaveChangesAsync();
            return vehicle;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
                return false;

            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Vehicle>> GetVehiclesByDriverId(Guid driverId)
        {
            return await _context.Vehicles
                .Where(v => v.AssignedDriverId == driverId)
                .ToListAsync();
        }

        public async Task<IEnumerable<MaintenanceRecord>> GetMaintenanceHistoryAsync(Guid vehicleId)
        {
            return await _context.MaintenanceRecords
                .Where(m => m.VehicleId == vehicleId)
                .OrderByDescending(m => m.CompletedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<FuelRecord>> GetFuelHistoryAsync(Guid vehicleId)
        {
            return await _context.FuelRecords
                .Where(f => f.VehicleId == vehicleId)
                .OrderByDescending(f => f.Timestamp)
                .ToListAsync();
        }
    }
}
