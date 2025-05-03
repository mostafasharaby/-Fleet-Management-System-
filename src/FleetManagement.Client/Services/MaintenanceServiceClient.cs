using AutoMapper;
using Grpc.Core;
using Grpc.Net.Client;
using MaintenanceService.API.Protos;

namespace FleetManagement.Client.Services
{
    public class MaintenanceServiceClient : IDisposable
    {
        private readonly GrpcChannel _channel;
        private readonly MaintenanceService.API.Protos.MaintenanceService.MaintenanceServiceClient _client;
        private readonly ILogger<MaintenanceServiceClient> _logger;
        private readonly IMapper _mapper;

        public MaintenanceServiceClient(string serviceUrl, ILogger<MaintenanceServiceClient> logger, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _channel = GrpcChannel.ForAddress(serviceUrl);
            _client = new MaintenanceService.API.Protos.MaintenanceService.MaintenanceServiceClient(_channel);
            _logger.LogInformation($"MaintenanceServiceClient initialized with endpoint: {serviceUrl}");
        }

        public async Task<List<MaintenanceTask>> GetMaintenanceScheduleAsync(Guid vehicleId, bool includeCompleted = false)
        {
            try
            {
                _logger.LogDebug($"Getting maintenance schedule for vehicle ID: {vehicleId}, IncludeCompleted: {includeCompleted}");
                var request = new GetMaintenanceScheduleRequest
                {
                    VehicleId = vehicleId.ToString(),
                    IncludeCompleted = includeCompleted
                };
                var response = await _client.GetMaintenanceScheduleAsync(request);
                return new List<MaintenanceTask>(response.ScheduledTasks);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogWarning($"Invalid vehicle ID format: {vehicleId}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting maintenance schedule for vehicle {vehicleId}");
                throw;
            }
        }

        public async Task<(bool Success, string EventId, string Message)> RecordMaintenanceEventAsync(MaintenanceEvent maintenanceEvent)
        {
            try
            {
                _logger.LogDebug($"Recording maintenance event for vehicle ID: {maintenanceEvent.VehicleId}");
                var request = new MaintenanceEventRequest
                {
                    Event = _mapper.Map<MaintenanceEvent>(maintenanceEvent)
                };
                var response = await _client.RecordMaintenanceEventAsync(request);
                return (response.Success, response.EventId, response.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error recording maintenance event for vehicle {maintenanceEvent.VehicleId}");
                throw;
            }
        }

        public async Task<(bool Success, string TaskId, string Message)> ScheduleMaintenanceAsync(MaintenanceTask task)
        {
            try
            {
                _logger.LogDebug($"Scheduling maintenance task: {task.TaskDescription}");
                var request = new ScheduleMaintenanceRequest
                {
                    Task = _mapper.Map<MaintenanceTask>(task)
                };
                var response = await _client.ScheduleMaintenanceAsync(request);
                return (response.Success, response.TaskId, response.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error scheduling maintenance task: {task.TaskDescription}");
                throw;
            }
        }

        public async Task<(bool Success, string Message)> UpdateMaintenanceStatusAsync(
            Guid taskId,
            MaintenanceStatus newStatus,
            string updatedBy,
            string notes = null)
        {
            try
            {
                _logger.LogDebug($"Updating maintenance status for task ID: {taskId}");
                var request = new UpdateMaintenanceStatusRequest
                {
                    TaskId = taskId.ToString(),
                    NewStatus = newStatus,
                    UpdatedBy = updatedBy,
                    Notes = notes ?? string.Empty
                };
                var response = await _client.UpdateMaintenanceStatusAsync(request);
                return (response.Success, response.Message);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogWarning($"Invalid task ID format: {taskId}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating maintenance status for task {taskId}");
                throw;
            }
        }

        public async Task<(List<MaintenanceEvent> Events, int TotalCount, int TotalPages)> GetMaintenanceHistoryAsync(
            Guid vehicleId,
            DateTime startDate,
            DateTime endDate,
            int pageSize = 10,
            int pageNumber = 1)
        {
            try
            {
                _logger.LogDebug($"Getting maintenance history for vehicle ID: {vehicleId}, from {startDate} to {endDate}");
                var request = new GetMaintenanceHistoryRequest
                {
                    VehicleId = vehicleId.ToString(),
                    StartDate = new DateTimeOffset(startDate).ToUnixTimeSeconds(),
                    EndDate = new DateTimeOffset(endDate).ToUnixTimeSeconds(),
                    PageSize = pageSize,
                    PageNumber = pageNumber
                };
                var response = await _client.GetMaintenanceHistoryAsync(request);
                return (new List<MaintenanceEvent>(response.Events), response.TotalCount, response.TotalPages);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogWarning($"Invalid vehicle ID format: {vehicleId}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting maintenance history for vehicle {vehicleId}");
                throw;
            }
        }

        public async Task<VehicleHealthMetricsResponse> GetVehicleHealthMetricsAsync(Guid vehicleId)
        {
            try
            {
                _logger.LogDebug($"Getting vehicle health metrics for vehicle ID: {vehicleId}");
                var request = new VehicleHealthMetricsRequest
                {
                    VehicleId = vehicleId.ToString()
                };
                return await _client.GetVehicleHealthMetricsAsync(request);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogWarning($"Invalid vehicle ID format: {vehicleId}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting vehicle health metrics for vehicle {vehicleId}");
                throw;
            }
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
}