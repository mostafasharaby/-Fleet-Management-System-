using FleetManagement.Client.Services;
using Grpc.Core;
using VehicleService.API.Protos;

namespace FleetManagement.Client.Workers
{
    public class VehicleWorker : BackgroundService
    {
        private readonly ILogger<VehicleWorker> _logger;
        private readonly VehicleServiceClient _vehicleServiceClient;

        public VehicleWorker(VehicleServiceClient vehicleServiceClient, ILogger<VehicleWorker> logger)
        {
            _vehicleServiceClient = vehicleServiceClient;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!stoppingToken.IsCancellationRequested)
            {
                //await GetVehiclesByID();
                //await GetListOfVehicles();
                //await CreateVehicle();
                //await UpdateVehicle();
                //await DeleteVehicle();
                //await AssignVehicleToDriver();
                //await StreamVehicleLocation(stoppingToken);
            }
        }

        private async Task GetVehiclesByID()
        {
            Guid vehicleId = Guid.Parse("8DB05DA5-9AE5-46D7-BC3C-10F260EAB20C");

            try
            {
                _logger.LogInformation($"Fetching vehicle with ID: {vehicleId}");
                VehicleResponse vehicle = await _vehicleServiceClient.GetVehicleAsync(vehicleId);

                if (vehicle != null)
                {
                    Console.WriteLine($"Vehicle Found: ID={vehicleId}");
                    Console.WriteLine($"Registration: {vehicle.RegistrationNumber}");
                    Console.WriteLine($"Model: {vehicle.Model}");
                    Console.WriteLine($"Manufacturer: {vehicle.Manufacturer}");
                    Console.WriteLine($"Year: {vehicle.Year}");
                }
                else
                {
                    Console.WriteLine($"Vehicle with ID {vehicleId} not found.");
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                _logger.LogWarning($"Vehicle with ID {vehicleId} not found: {ex.Message}");
                Console.WriteLine($"Vehicle with ID {vehicleId} not found: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving vehicle with ID {vehicleId}");
                Console.WriteLine($"Error retrieving vehicle: {ex.Message}");
            }
        }

        private async Task GetListOfVehicles()
        {
            try
            {
                _logger.LogInformation("Fetching list of vehicles");
                var (vehicles, totalCount, pageCount) = await _vehicleServiceClient.ListVehiclesAsync(pageSize: 10, pageNumber: 1);

                Console.WriteLine($"Total Vehicles: {totalCount}");
                Console.WriteLine($"Page Count: {pageCount}");

                foreach (var vehicle in vehicles)
                {
                    Console.WriteLine($"Vehicle Found: ID={vehicle.Id}");
                    Console.WriteLine($"Registration: {vehicle.RegistrationNumber}");
                    Console.WriteLine($"Model: {vehicle.Model}");
                    Console.WriteLine($"Manufacturer: {vehicle.Manufacturer}");
                    Console.WriteLine($"Year: {vehicle.Year}");
                    Console.WriteLine("---");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing vehicles");
                Console.WriteLine($"Error listing vehicles: {ex.Message}");
            }
        }

        private async Task CreateVehicle()
        {
            try
            {
                _logger.LogInformation("Creating a new vehicle");
                var vehicle = await _vehicleServiceClient.CreateVehicleAsync(
                    registrationNumber: "ABC123",
                    model: "Model 3",
                    manufacturer: "Tesla",
                    year: 2023,
                    vin: "5YJSA1E29NF123456",
                    type: VehicleType.Sedan,
                    fuelCapacity: 75.0f,
                    currentFuelLevel: 50.0f,
                    odometerReading: 10000.0f
                );

                if (vehicle != null)
                {
                    Console.WriteLine($"Vehicle Created: ID={vehicle.Id}");
                    Console.WriteLine($"Registration: {vehicle.RegistrationNumber}");
                    Console.WriteLine($"Model: {vehicle.Model}");
                    Console.WriteLine($"Manufacturer: {vehicle.Manufacturer}");
                    Console.WriteLine($"Year: {vehicle.Year}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating vehicle");
                Console.WriteLine($"Error creating vehicle: {ex.Message}");
            }
        }

        private async Task UpdateVehicle()
        {
            Guid vehicleId = Guid.Parse("8DB05DA5-9AE5-46D7-BC3C-10F260EAB20C");

            try
            {
                _logger.LogInformation($"Updating vehicle with ID: {vehicleId}");
                var vehicle = await _vehicleServiceClient.UpdateVehicleAsync(
                    vehicleId: vehicleId,
                    registrationNumber: "XYZ789",
                    model: "Model S",
                    manufacturer: "Tesla",
                    year: 2024,
                    vin: "5YJSA1E29NF123456",
                    type: VehicleType.Sedan,
                    fuelCapacity: 80.0f,
                    currentFuelLevel: 60.0f,
                    odometerReading: 15000.0f,
                    status: VehicleStatus.Active
                );

                if (vehicle != null)
                {
                    Console.WriteLine($"Vehicle Updated: ID={vehicle.Id}");
                    Console.WriteLine($"Registration: {vehicle.RegistrationNumber}");
                    Console.WriteLine($"Model: {vehicle.Model}");
                    Console.WriteLine($"Manufacturer: {vehicle.Manufacturer}");
                    Console.WriteLine($"Year: {vehicle.Year}");
                }
                else
                {
                    Console.WriteLine($"Vehicle with ID {vehicleId} not found for update.");
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                _logger.LogWarning($"Vehicle with ID {vehicleId} not found for update");
                Console.WriteLine($"Vehicle with ID {vehicleId} not found: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating vehicle with ID {vehicleId}");
                Console.WriteLine($"Error updating vehicle: {ex.Message}");
            }
        }

        private async Task DeleteVehicle()
        {
            Guid vehicleId = Guid.Parse("8DB05DA5-9AE5-46D7-BC3C-10F260EAB20C");

            try
            {
                _logger.LogInformation($"Deleting vehicle with ID: {vehicleId}");
                bool success = await _vehicleServiceClient.DeleteVehicleAsync(vehicleId);

                if (success)
                {
                    Console.WriteLine($"Vehicle with ID {vehicleId} deleted successfully.");
                }
                else
                {
                    Console.WriteLine($"Vehicle with ID {vehicleId} not found for deletion.");
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                _logger.LogWarning($"Vehicle with ID {vehicleId} not found for deletion");
                Console.WriteLine($"Vehicle with ID {vehicleId} not found: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting vehicle with ID {vehicleId}");
                Console.WriteLine($"Error deleting vehicle: {ex.Message}");
            }
        }

        private async Task AssignVehicleToDriver()
        {
            Guid vehicleId = Guid.Parse("8DB05DA5-9AE5-46D7-BC3C-10F260EAB20C");
            Guid driverId = Guid.NewGuid();
            DateTime assignmentStart = DateTime.UtcNow;
            DateTime? assignmentEnd = DateTime.UtcNow.AddDays(7);

            try
            {
                _logger.LogInformation($"Assigning vehicle {vehicleId} to driver {driverId}");
                var vehicle = await _vehicleServiceClient.AssignVehicleToDriverAsync(
                    vehicleId: vehicleId,
                    driverId: driverId,
                    assignmentStart: assignmentStart,
                    assignmentEnd: assignmentEnd
                );

                if (vehicle != null)
                {
                    Console.WriteLine($"Vehicle Assigned: ID={vehicle.Id}");
                    Console.WriteLine($"Driver ID: {driverId}");
                    Console.WriteLine($"Assignment Start: {assignmentStart}");
                    if (assignmentEnd.HasValue)
                    {
                        Console.WriteLine($"Assignment End: {assignmentEnd.Value}");
                    }
                }
                else
                {
                    Console.WriteLine($"Vehicle with ID {vehicleId} not found for assignment.");
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                _logger.LogWarning($"Vehicle with ID {vehicleId} not found for assignment");
                Console.WriteLine($"Vehicle with ID {vehicleId} not found: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error assigning vehicle {vehicleId} to driver {driverId}");
                Console.WriteLine($"Error assigning vehicle: {ex.Message}");
            }
        }

        private async Task StreamVehicleLocation(CancellationToken cancellationToken)
        {
            Guid vehicleId = Guid.Parse("8DB05DA5-9AE5-46D7-BC3C-10F260EAB20C");

            try
            {
                _logger.LogInformation($"Streaming location for vehicle {vehicleId}");
                await foreach (var location in _vehicleServiceClient.StreamVehicleLocationAsync(vehicleId, cancellationToken))
                {
                    Console.WriteLine($"Vehicle Location Update: ID={vehicleId}");
                    Console.WriteLine($"Latitude: {location.Latitude}");
                    Console.WriteLine($"Longitude: {location.Longitude}");
                    Console.WriteLine($"Timestamp: {location.Timestamp}");
                    Console.WriteLine("---");
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                _logger.LogInformation("Streaming cancelled");
                Console.WriteLine("Location streaming cancelled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error streaming location for vehicle {vehicleId}");
                Console.WriteLine($"Error streaming location: {ex.Message}");
            }
        }
    }
}