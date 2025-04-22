using Microsoft.Extensions.Logging;
using VehicleService.Domain.Enums;
using VehicleService.Domain.Models;
using VehicleService.Domain.Repositories;

namespace VehicleService.Application.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly ILogger<VehicleService> _logger;

        public VehicleService(IVehicleRepository vehicleRepository, ILogger<VehicleService> logger)
        {
            _vehicleRepository = vehicleRepository;
            _logger = logger;
        }

        public async Task<Vehicle> GetVehicleAsync(Guid id)
        {
            return await _vehicleRepository.GetByIdAsync(id);
        }

        public async Task<(IEnumerable<Vehicle> Vehicles, int TotalCount, int PageCount)> ListVehiclesAsync(
            int pageSize, int pageNumber, string filter = null, VehicleStatus? status = null)
        {
            var vehicles = await _vehicleRepository.ListAsync(pageSize, pageNumber, filter, status);
            var count = await _vehicleRepository.CountAsync(filter, status);
            var pageCount = (int)Math.Ceiling((double)count / pageSize);

            return (vehicles, count, pageCount);
        }

        public async Task<Vehicle> CreateVehicleAsync(
            string registrationNumber,
            string model,
            string manufacturer,
            int year,
            string vin,
            VehicleType type,
            float fuelCapacity,
            float currentFuelLevel,
            float odometerReading)
        {
            var vehicle = Vehicle.Create(
                registrationNumber,
                model,
                manufacturer,
                year,
                vin,
                type,
                fuelCapacity,
                currentFuelLevel,
                odometerReading);

            await _vehicleRepository.AddAsync(vehicle);
            _logger.LogInformation($"Created new vehicle with ID {vehicle.Id}");
            return vehicle;
        }

        public async Task<Vehicle> UpdateVehicleAsync(
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
            VehicleStatus status)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            if (vehicle == null)
            {
                _logger.LogWarning($"Failed to update vehicle: Vehicle with ID {id} not found");
                return null;
            }

            vehicle.Update(
                registrationNumber,
                model,
                manufacturer,
                year,
                vin,
                type,
                fuelCapacity,
                currentFuelLevel,
                odometerReading,
                status);

            await _vehicleRepository.UpdateAsync(vehicle);
            _logger.LogInformation($"Updated vehicle with ID {vehicle.Id}");
            return vehicle;
        }

        public async Task<bool> DeleteVehicleAsync(Guid id)
        {
            var result = await _vehicleRepository.DeleteAsync(id);
            if (result)
            {
                _logger.LogInformation($"Deleted vehicle with ID {id}");
            }
            else
            {
                _logger.LogWarning($"Failed to delete vehicle: Vehicle with ID {id} not found");
            }
            return result;
        }

        public async Task<Vehicle> AssignVehicleToDriverAsync(Guid vehicleId, Guid driverId, DateTime? endDate = null)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
            if (vehicle == null)
            {
                _logger.LogWarning($"Failed to assign vehicle: Vehicle with ID {vehicleId} not found");
                return null;
            }

            vehicle.AssignDriver(driverId, endDate);
            await _vehicleRepository.UpdateAsync(vehicle);
            _logger.LogInformation($"Assigned vehicle {vehicleId} to driver {driverId}");
            return vehicle;
        }

        public async Task<Vehicle> UnassignVehicleFromDriverAsync(Guid vehicleId)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
            if (vehicle == null)
            {
                _logger.LogWarning($"Failed to unassign vehicle: Vehicle with ID {vehicleId} not found");
                return null;
            }

            vehicle.UnassignDriver();
            await _vehicleRepository.UpdateAsync(vehicle);
            _logger.LogInformation($"Unassigned driver from vehicle {vehicleId}");
            return vehicle;
        }

        public async Task<Vehicle> UpdateVehicleLocationAsync(Guid vehicleId, double latitude, double longitude, double speed, double heading)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
            if (vehicle == null)
            {
                _logger.LogWarning($"Failed to update location: Vehicle with ID {vehicleId} not found");
                return null;
            }

            vehicle.UpdateLocation(latitude, longitude, speed, heading);
            await _vehicleRepository.UpdateAsync(vehicle);
            _logger.LogDebug($"Updated location for vehicle {vehicleId}");
            return vehicle;
        }

        public async Task<IEnumerable<MaintenanceRecord>> GetMaintenanceHistoryAsync(Guid vehicleId)
        {
            return await _vehicleRepository.GetMaintenanceHistoryAsync(vehicleId);
        }

        public async Task<IEnumerable<FuelRecord>> GetFuelHistoryAsync(Guid vehicleId)
        {
            return await _vehicleRepository.GetFuelHistoryAsync(vehicleId);
        }
    }
}
