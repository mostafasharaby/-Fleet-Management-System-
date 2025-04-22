using VehicleService.Domain.Enums;
using VehicleService.Domain.Models;

namespace VehicleService.Application.Services
{
    public interface IVehicleService
    {
        Task<Vehicle> GetVehicleAsync(Guid id);
        Task<(IEnumerable<Vehicle> Vehicles, int TotalCount, int PageCount)> ListVehiclesAsync(
            int pageSize, int pageNumber, string filter = null, VehicleStatus? status = null);
        Task<Vehicle> CreateVehicleAsync(
            string registrationNumber,
            string model,
            string manufacturer,
            int year,
            string vin,
            VehicleType type,
            float fuelCapacity,
            float currentFuelLevel,
            float odometerReading);
        Task<Vehicle> UpdateVehicleAsync(
            Guid id,
            string registrationNumber,
            string model,
            string manufacturer,
            int year,
            string vin,
            VehicleType type,
            float fuelCapacity,
            float currentFuelLevel,
            float odometerReading,
            VehicleStatus status);
        Task<bool> DeleteVehicleAsync(Guid id);
        Task<Vehicle> AssignVehicleToDriverAsync(Guid vehicleId, Guid driverId, DateTime? endDate = null);
        Task<Vehicle> UnassignVehicleFromDriverAsync(Guid vehicleId);
        Task<Vehicle> UpdateVehicleLocationAsync(Guid vehicleId, double latitude, double longitude, double speed, double heading);
        Task<IEnumerable<MaintenanceRecord>> GetMaintenanceHistoryAsync(Guid vehicleId);
        Task<IEnumerable<FuelRecord>> GetFuelHistoryAsync(Guid vehicleId);
    }
}
