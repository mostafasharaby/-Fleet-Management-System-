using Grpc.Core;
using Grpc.Net.Client;
using VehicleService.API.Protos;
namespace FleetManagement.Client.Services
{
    public class VehicleServiceClient : IDisposable
    {
        private readonly GrpcChannel _channel;
        private readonly VehicleService.API.Protos.VehicleService.VehicleServiceClient _client;
        private readonly ILogger<VehicleServiceClient> _logger;

        public VehicleServiceClient(string serviceUrl, ILogger<VehicleServiceClient> logger)
        {
            _logger = logger;
            _channel = GrpcChannel.ForAddress(serviceUrl);
            _client = new VehicleService.API.Protos.VehicleService.VehicleServiceClient(_channel);
            _logger.LogInformation($"VehicleServiceClient initialized with endpoint: {serviceUrl}");
        }


        public async Task<VehicleResponse> GetVehicleAsync(Guid vehicleId)
        {
            vehicleId = Guid.Parse("8DB05DA5-9AE5-46D7-BC3C-10F260EAB20C");
            try
            {
                _logger.LogDebug($"Getting vehicle with ID: {vehicleId}");

                var request = new GetVehicleRequest
                {
                    VehicleId = vehicleId.ToString()
                };

                return await _client.GetVehicleAsync(request);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                _logger.LogWarning($"Vehicle with ID {vehicleId} not found");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting vehicle with ID {vehicleId}");
                throw;
            }
        }
        public async Task<(List<VehicleResponse> Vehicles, int TotalCount, int PageCount)> ListVehiclesAsync(
            int pageSize = 10,
            int pageNumber = 1,
            string filter = null,
            VehicleStatus? status = null)
        {
            try
            {
                _logger.LogDebug($"Listing vehicles: Page {pageNumber}, Size {pageSize}, Filter: {filter}, Status: {status}");

                var request = new ListVehiclesRequest
                {
                    PageSize = pageSize,
                    PageNumber = pageNumber,
                    Filter = filter ?? string.Empty
                };

                if (status.HasValue)
                {
                    request.Status = status.Value;
                }

                var response = await _client.ListVehiclesAsync(request);

                return (new List<VehicleResponse>(response.Vehicles), response.TotalCount, response.PageCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing vehicles");
                throw;
            }
        }


        public async Task<VehicleResponse> CreateVehicleAsync(
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
            try
            {
                _logger.LogDebug($"Creating vehicle: {registrationNumber}, {manufacturer} {model}");

                var request = new CreateVehicleRequest
                {
                    RegistrationNumber = registrationNumber,
                    Model = model,
                    Manufacturer = manufacturer,
                    Year = year,
                    Vin = vin,
                    Type = type,
                    FuelCapacity = fuelCapacity,
                    CurrentFuelLevel = currentFuelLevel,
                    OdometerReading = odometerReading
                };

                return await _client.CreateVehicleAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating vehicle: {registrationNumber}");
                throw;
            }
        }


        public async Task<VehicleResponse> UpdateVehicleAsync(
            Guid vehicleId,
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
            try
            {
                _logger.LogDebug($"Updating vehicle with ID: {vehicleId}");

                var request = new UpdateVehicleRequest
                {
                    VehicleId = vehicleId.ToString(),
                    RegistrationNumber = registrationNumber,
                    Model = model,
                    Manufacturer = manufacturer,
                    Year = year,
                    Vin = vin,
                    Type = type,
                    FuelCapacity = fuelCapacity,
                    CurrentFuelLevel = currentFuelLevel,
                    OdometerReading = odometerReading,
                    Status = status
                };

                return await _client.UpdateVehicleAsync(request);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                _logger.LogWarning($"Vehicle with ID {vehicleId} not found during update");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating vehicle with ID {vehicleId}");
                throw;
            }
        }


        public async Task<bool> DeleteVehicleAsync(Guid vehicleId)
        {
            try
            {
                _logger.LogDebug($"Deleting vehicle with ID: {vehicleId}");

                var request = new DeleteVehicleRequest
                {
                    VehicleId = vehicleId.ToString()
                };

                var response = await _client.DeleteVehicleAsync(request);
                return response.Success;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                _logger.LogWarning($"Vehicle with ID {vehicleId} not found during deletion");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting vehicle with ID {vehicleId}");
                throw;
            }
        }

        public async Task<VehicleResponse> AssignVehicleToDriverAsync(
            Guid vehicleId,
            Guid driverId,
            DateTime assignmentStart,
            DateTime? assignmentEnd = null)
        {
            try
            {
                _logger.LogDebug($"Assigning vehicle {vehicleId} to driver {driverId}");

                var request = new AssignVehicleRequest
                {
                    VehicleId = vehicleId.ToString(),
                    DriverId = driverId.ToString(),
                    AssignmentStartTimestamp = new DateTimeOffset(assignmentStart).ToUnixTimeSeconds()
                };

                if (assignmentEnd.HasValue)
                {
                    request.AssignmentEndTimestamp = new DateTimeOffset(assignmentEnd.Value).ToUnixTimeSeconds();
                }

                return await _client.AssignVehicleToDriverAsync(request);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                _logger.LogWarning($"Vehicle with ID {vehicleId} not found during driver assignment");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error assigning vehicle {vehicleId} to driver {driverId}");
                throw;
            }
        }


        public async IAsyncEnumerable<VehicleLocationResponse> StreamVehicleLocationAsync(
            Guid vehicleId,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            _logger.LogDebug($"Starting location stream for vehicle: {vehicleId}");

            var request = new VehicleLocationStreamRequest
            {
                VehicleId = vehicleId.ToString()
            };

            using var call = _client.StreamVehicleLocation(request, cancellationToken: cancellationToken);

            await foreach (var locationUpdate in call.ResponseStream.ReadAllAsync(cancellationToken))
            {
                yield return locationUpdate;
            }
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
}
