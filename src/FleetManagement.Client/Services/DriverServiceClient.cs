using DriverService.API.Protos;
using Grpc.Core;
using Grpc.Net.Client;

namespace FleetManagement.Client.Services
{
    public class DriverServiceClient : IDisposable
    {
        private readonly GrpcChannel _channel;
        private readonly DriverService.API.Protos.DriverService.DriverServiceClient _client;
        private readonly ILogger<DriverServiceClient> _logger;

        public DriverServiceClient(string serviceUrl, ILogger<DriverServiceClient> logger)
        {
            _logger = logger;
            _channel = GrpcChannel.ForAddress(serviceUrl);
            _client = new DriverService.API.Protos.DriverService.DriverServiceClient(_channel);
            _logger.LogInformation($"DriverServiceClient initialized with endpoint: {serviceUrl}");
        }

        public async Task<DriverResponse> GetDriverAsync(Guid driverId)
        {
            try
            {
                _logger.LogDebug($"Getting driver with ID: {driverId}");
                var request = new GetDriverRequest { DriverId = driverId.ToString() };
                return await _client.GetDriverAsync(request);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                _logger.LogWarning($"Driver with ID {driverId} not found");
                return null;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogWarning($"Invalid driver ID format: {driverId}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting driver with ID {driverId}");
                throw;
            }
        }

        public async Task<(List<DriverResponse> Drivers, int TotalCount, int PageCount)> ListDriversAsync(
            int pageSize = 10,
            int pageNumber = 1,
            string filter = null,
            DriverStatus? status = null)
        {
            try
            {
                _logger.LogDebug($"Listing drivers: Page {pageNumber}, Size {pageSize}, Filter: {filter}, Status: {status}");
                var request = new ListDriversRequest
                {
                    PageSize = pageSize,
                    PageNumber = pageNumber,
                    Filter = filter ?? string.Empty
                };

                if (status.HasValue)
                {
                    request.Status = status.Value;
                }

                var response = await _client.ListDriversAsync(request);
                return (new List<DriverResponse>(response.Drivers), response.TotalCount, response.PageCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing drivers");
                throw;
            }
        }

        public async Task<DriverResponse> CreateDriverAsync(
            string firstName,
            string lastName,
            string email,
            string phoneNumber,
            string licenseNumber,
            string licenseState,
            DateTime licenseExpiry,
            DriverType type)
        {
            try
            {
                _logger.LogDebug($"Creating driver: {firstName} {lastName}");
                var request = new CreateDriverRequest
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    LicenseNumber = licenseNumber,
                    LicenseState = licenseState,
                    LicenseExpiry = new DateTimeOffset(licenseExpiry).ToUnixTimeSeconds(),
                    Type = type
                };

                return await _client.CreateDriverAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating driver: {firstName} {lastName}");
                throw;
            }
        }

        public async Task<DriverResponse> UpdateDriverAsync(
            Guid driverId,
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
            try
            {
                _logger.LogDebug($"Updating driver with ID: {driverId}");
                var request = new UpdateDriverRequest
                {
                    DriverId = driverId.ToString(),
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    LicenseNumber = licenseNumber,
                    LicenseState = licenseState,
                    LicenseExpiry = new DateTimeOffset(licenseExpiry).ToUnixTimeSeconds(),
                    Status = status,
                    Type = type
                };

                return await _client.UpdateDriverAsync(request);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                _logger.LogWarning($"Driver with ID {driverId} not found during update");
                return null;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogWarning($"Invalid driver ID format: {driverId}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating driver with ID {driverId}");
                throw;
            }
        }

        public async Task<(bool Success, string Message)> DeleteDriverAsync(Guid driverId)
        {
            try
            {
                _logger.LogDebug($"Deleting driver with ID: {driverId}");
                var request = new DeleteDriverRequest { DriverId = driverId.ToString() };
                var response = await _client.DeleteDriverAsync(request);
                return (response.Success, response.Message);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogWarning($"Invalid driver ID format: {driverId}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting driver with ID {driverId}");
                throw;
            }
        }

        public async Task<List<AssignmentResponse>> GetDriverAssignmentsAsync(Guid driverId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                _logger.LogDebug($"Getting assignments for driver {driverId} from {fromDate} to {toDate}");
                var request = new GetDriverAssignmentsRequest
                {
                    DriverId = driverId.ToString(),
                    FromDate = new DateTimeOffset(fromDate).ToUnixTimeSeconds(),
                    ToDate = new DateTimeOffset(toDate).ToUnixTimeSeconds()
                };

                var response = await _client.GetDriverAssignmentsAsync(request);
                return new List<AssignmentResponse>(response.Assignments);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogWarning($"Invalid driver ID format: {driverId}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting assignments for driver {driverId}");
                throw;
            }
        }

        public async Task<AssignmentResponse> AssignVehicleAsync(
            Guid driverId,
            Guid vehicleId,
            DateTime startTime,
            DateTime endTime,
            string notes = null)
        {
            try
            {
                _logger.LogDebug($"Assigning vehicle {vehicleId} to driver {driverId}");
                var request = new AssignVehicleRequest
                {
                    DriverId = driverId.ToString(),
                    VehicleId = vehicleId.ToString(),
                    StartTime = new DateTimeOffset(startTime).ToUnixTimeSeconds(),
                    EndTime = new DateTimeOffset(endTime).ToUnixTimeSeconds(),
                    Notes = notes ?? string.Empty
                };

                return await _client.AssignVehicleAsync(request);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                _logger.LogWarning($"Driver or vehicle not found: Driver ID {driverId}, Vehicle ID {vehicleId}");
                return null;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogWarning($"Invalid ID format: Driver ID {driverId}, Vehicle ID {vehicleId}");
                throw;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.FailedPrecondition)
            {
                _logger.LogWarning($"Assignment failed: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error assigning vehicle {vehicleId} to driver {driverId}");
                throw;
            }
        }

        public async Task<AssignmentResponse> CompleteAssignmentAsync(
            Guid assignmentId,
            float finalOdometer,
            float fuelLevel,
            string notes = null)
        {
            try
            {
                _logger.LogDebug($"Completing assignment {assignmentId}");
                var request = new CompleteAssignmentRequest
                {
                    AssignmentId = assignmentId.ToString(),
                    FinalOdometer = finalOdometer,
                    FuelLevel = fuelLevel,
                    Notes = notes ?? string.Empty
                };

                return await _client.CompleteAssignmentAsync(request);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                _logger.LogWarning($"Assignment with ID {assignmentId} not found");
                return null;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogWarning($"Invalid assignment ID format: {assignmentId}");
                throw;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.FailedPrecondition)
            {
                _logger.LogWarning($"Assignment completion failed: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error completing assignment {assignmentId}");
                throw;
            }
        }

        public async Task<List<AvailabilitySlot>> GetDriverAvailabilityAsync(Guid driverId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                _logger.LogDebug($"Getting availability for driver {driverId} from {fromDate} to {toDate}");
                var request = new GetDriverAvailabilityRequest
                {
                    DriverId = driverId.ToString(),
                    FromDate = new DateTimeOffset(fromDate).ToUnixTimeSeconds(),
                    ToDate = new DateTimeOffset(toDate).ToUnixTimeSeconds()
                };

                var response = await _client.GetDriverAvailabilityAsync(request);
                return new List<AvailabilitySlot>(response.AvailabilitySlots);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogWarning($"Invalid driver ID format: {driverId}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting availability for driver {driverId}");
                throw;
            }
        }

        public async Task<(bool Success, string Message, List<ScheduleConflict> Conflicts)> ScheduleDriverAsync(
            Guid driverId,
            List<(DateTime StartTime, DateTime EndTime, ScheduleType Type, string Notes)> scheduleSlots)
        {
            try
            {
                _logger.LogDebug($"Scheduling driver {driverId} with {scheduleSlots.Count} slots");
                var request = new ScheduleDriverRequest { DriverId = driverId.ToString() };

                foreach (var slot in scheduleSlots)
                {
                    request.ScheduleSlots.Add(new ScheduleSlot
                    {
                        StartTime = new DateTimeOffset(slot.StartTime).ToUnixTimeSeconds(),
                        EndTime = new DateTimeOffset(slot.EndTime).ToUnixTimeSeconds(),
                        Type = slot.Type,
                        Notes = slot.Notes ?? string.Empty
                    });
                }

                var response = await _client.ScheduleDriverAsync(request);
                return (response.Success, response.Message, new List<ScheduleConflict>(response.Conflicts));
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogWarning($"Invalid driver ID format: {driverId}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error scheduling driver {driverId}");
                throw;
            }
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
}