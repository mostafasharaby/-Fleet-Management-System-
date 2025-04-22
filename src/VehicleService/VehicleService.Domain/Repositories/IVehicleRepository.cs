using VehicleService.Domain.Enums;
using VehicleService.Domain.Models;

namespace VehicleService.Domain.Repositories
{
    public interface IVehicleRepository
    {
        Task<Vehicle> GetByIdAsync(Guid id);
        Task<IEnumerable<Vehicle>> ListAsync(int pageSize, int pageNumber, string filter = null, VehicleStatus? status = null);
        Task<int> CountAsync(string filter = null, VehicleStatus? status = null);
        Task<Vehicle> AddAsync(Vehicle vehicle);
        Task<Vehicle> UpdateAsync(Vehicle vehicle);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<Vehicle>> GetVehiclesByDriverId(Guid driverId);
        Task<IEnumerable<MaintenanceRecord>> GetMaintenanceHistoryAsync(Guid vehicleId);
        Task<IEnumerable<FuelRecord>> GetFuelHistoryAsync(Guid vehicleId);
    }
}
