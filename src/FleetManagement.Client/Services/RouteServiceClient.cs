using AutoMapper;
using Grpc.Core;
using Grpc.Net.Client;
using RouteService.API.Protos;
using RouteService.Domain.Models;

namespace FleetManagement.Client.Services
{
    public class RouteServiceClient : IDisposable
    {
        private readonly GrpcChannel _channel;
        private readonly RouteService.API.Protos.RouteService.RouteServiceClient _client;
        private readonly ILogger<RouteServiceClient> _logger;
        private readonly IMapper _mapper;

        public RouteServiceClient(string serviceUrl, ILogger<RouteServiceClient> logger, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _channel = GrpcChannel.ForAddress(serviceUrl);
            _client = new RouteService.API.Protos.RouteService.RouteServiceClient(_channel);
            _logger.LogInformation($"RouteServiceClient initialized with endpoint: {serviceUrl}");
        }

        public async Task<RouteResponse> GetRouteAsync(Guid routeId)
        {
            try
            {
                _logger.LogDebug($"Getting route with ID: {routeId}");
                var request = new GetRouteRequest { RouteId = routeId.ToString() };
                return await _client.GetRouteAsync(request);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                _logger.LogWarning($"Route with ID {routeId} not found");
                return null;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogWarning($"Invalid route ID format: {routeId}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting route with ID {routeId}");
                throw;
            }
        }

        public async Task<(List<RouteMessage> Routes, int TotalCount, int PageCount)> GetAllRoutesAsync(
            int pageSize = 10,
            int pageNumber = 1,
            string filter = null,
            RouteStatus? status = null)
        {
            try
            {
                _logger.LogDebug($"Listing routes: Page {pageNumber}, Size {pageSize}, Filter: {filter}, Status: {status}");
                var request = new GetAllRoutesRequest
                {
                    PageSize = pageSize,
                    PageNumber = pageNumber,
                    Filter = filter ?? string.Empty
                };

                if (status.HasValue)
                {
                    request.Status = status.Value;
                }

                var response = await _client.GetAllRoutesAsync(request);
                return (new List<RouteMessage>(response.Routes), response.TotalCount, response.PageCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing routes");
                throw;
            }
        }

        public async Task<List<RouteMessage>> GetRoutesByVehicleAsync(Guid vehicleId)
        {
            try
            {
                _logger.LogDebug($"Getting routes for vehicle ID: {vehicleId}");
                var request = new GetRoutesByVehicleRequest { VehicleId = vehicleId.ToString() };
                var response = await _client.GetRoutesByVehicleAsync(request);
                return new List<RouteMessage>(response.Routes);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogWarning($"Invalid vehicle ID format: {vehicleId}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting routes for vehicle {vehicleId}");
                throw;
            }
        }

        public async Task<List<RouteMessage>> GetRoutesByDriverAsync(Guid driverId)
        {
            try
            {
                _logger.LogDebug($"Getting routes for driver ID: {driverId}");
                var request = new GetRoutesByDriverRequest { DriverId = driverId.ToString() };
                var response = await _client.GetRoutesByDriverAsync(request);
                return new List<RouteMessage>(response.Routes);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogWarning($"Invalid driver ID format: {driverId}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting routes for driver {driverId}");
                throw;
            }
        }

        public async Task<List<RouteMessage>> GetRoutesByStatusAsync(string status)
        {
            try
            {
                _logger.LogDebug($"Getting routes with status: {status}");
                var request = new GetRoutesByStatusRequest { Status = status };
                var response = await _client.GetRoutesByStatusAsync(request);
                return new List<RouteMessage>(response.Routes);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogWarning($"Invalid status: {status}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting routes with status {status}");
                throw;
            }
        }

        public async Task<List<RouteMessage>> GetRoutesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                _logger.LogDebug($"Getting routes from {startDate} to {endDate}");
                var request = new GetRoutesByDateRangeRequest
                {
                    StartDate = startDate.ToString("o"),
                    EndDate = endDate.ToString("o")
                };
                var response = await _client.GetRoutesByDateRangeAsync(request);
                return new List<RouteMessage>(response.Routes);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogWarning($"Invalid date format: Start={startDate}, End={endDate}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting routes from {startDate} to {endDate}");
                throw;
            }
        }

        public async Task<RouteResponse> CreateRouteAsync(
            string name,
            Guid vehicleId,
            Guid driverId,
            DateTime startTime,
            List<RouteStop> stops)
        {
            try
            {
                _logger.LogDebug($"Creating route: {name}");
                var request = new CreateRouteRequest
                {
                    Name = name,
                    VehicleId = vehicleId.ToString(),
                    DriverId = driverId.ToString(),
                    StartTime = startTime.ToString("o")
                };
                foreach (var stop in stops)
                {
                    request.Stops.Add(_mapper.Map<RouteStopMessage>(stops));
                }
                return await _client.CreateRouteAsync(request);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogWarning($"Invalid input for route creation: Name={name}, VehicleId={vehicleId}, DriverId={driverId}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating route: {name}");
                throw;
            }
        }

        public async Task<RouteResponse> UpdateRouteAsync(
            Guid routeId,
            string name,
            Guid vehicleId,
            Guid driverId,
            DateTime startTime)
        {
            try
            {
                _logger.LogDebug($"Updating route with ID: {routeId}");
                var request = new UpdateRouteRequest
                {
                    RouteId = routeId.ToString(),
                    Name = name,
                    VehicleId = vehicleId.ToString(),
                    DriverId = driverId.ToString(),
                    StartTime = startTime.ToString("o")
                };

                return await _client.UpdateRouteAsync(request);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                _logger.LogWarning($"Route with ID {routeId} not found");
                return null;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogWarning($"Invalid input for route update: ID={routeId}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating route with ID {routeId}");
                throw;
            }
        }

        public async Task<(bool Success, string Message)> DeleteRouteAsync(Guid routeId)
        {
            try
            {
                _logger.LogDebug($"Deleting route with ID: {routeId}");
                var request = new DeleteRouteRequest { RouteId = routeId.ToString() };
                var response = await _client.DeleteRouteAsync(request);
                return (response.Success, response.Message);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                _logger.LogWarning($"Route with ID {routeId} not found");
                return (false, $"Route with ID {routeId} not found");
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogWarning($"Invalid route ID format: {routeId}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting route with ID {routeId}");
                throw;
            }
        }

        public async Task<RouteResponse> OptimizeRouteAsync(Guid routeId)
        {
            try
            {
                _logger.LogDebug($"Optimizing route with ID: {routeId}");
                var request = new OptimizeRouteRequest { RouteId = routeId.ToString() };
                return await _client.OptimizeRouteAsync(request);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                _logger.LogWarning($"Route with ID {routeId} not found");
                return null;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogWarning($"Invalid route ID format: {routeId}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error optimizing route with ID {routeId}");
                throw;
            }
        }

        public async Task<RouteResponse> StartRouteAsync(Guid routeId)
        {
            try
            {
                _logger.LogDebug($"Starting route with ID: {routeId}");
                var request = new StartRouteRequest { RouteId = routeId.ToString() };
                return await _client.StartRouteAsync(request);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                _logger.LogWarning($"Route with ID {routeId} not found");
                return null;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogWarning($"Invalid route ID format: {routeId}");
                throw;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.FailedPrecondition)
            {
                _logger.LogWarning($"Cannot start route: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error starting route with ID {routeId}");
                throw;
            }
        }

        public async Task<RouteResponse> CompleteRouteAsync(Guid routeId)
        {
            try
            {
                _logger.LogDebug($"Completing route with ID: {routeId}");
                var request = new CompleteRouteRequest { RouteId = routeId.ToString() };
                return await _client.CompleteRouteAsync(request);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                _logger.LogWarning($"Route with ID {routeId} not found");
                return null;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogWarning($"Invalid route ID format: {routeId}");
                throw;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.FailedPrecondition)
            {
                _logger.LogWarning($"Cannot complete route: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error completing route with ID {routeId}");
                throw;
            }
        }

        public async Task<RouteResponse> CancelRouteAsync(Guid routeId, string reason)
        {
            try
            {
                _logger.LogDebug($"Cancelling route with ID: {routeId}");
                var request = new CancelRouteRequest
                {
                    RouteId = routeId.ToString(),
                    Reason = reason ?? string.Empty
                };
                return await _client.CancelRouteAsync(request);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                _logger.LogWarning($"Route with ID {routeId} not found");
                return null;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogWarning($"Invalid route ID format: {routeId}");
                throw;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.FailedPrecondition)
            {
                _logger.LogWarning($"Cannot cancel route: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error cancelling route with ID {routeId}");
                throw;
            }
        }

        public async Task<RouteResponse> AddStopToRouteAsync(Guid routeId, RouteStop stop)
        {
            try
            {
                _logger.LogDebug($"Adding stop to route with ID: {routeId}");
                var request = new AddStopToRouteRequest
                {
                    RouteId = routeId.ToString(),
                    Stop = _mapper.Map<RouteStopMessage>(stop)
                };
                return await _client.AddStopToRouteAsync(request);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                _logger.LogWarning($"Route with ID {routeId} not found");
                return null;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogWarning($"Invalid input for adding stop to route: ID={routeId}");
                throw;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.FailedPrecondition)
            {
                _logger.LogWarning($"Cannot add stop to route: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding stop to route with ID {routeId}");
                throw;
            }
        }

        public async Task<RouteResponse> UpdateStopStatusAsync(Guid routeId, Guid stopId)
        {
            try
            {
                _logger.LogDebug($"Updating stop status for stop {stopId} in route {routeId}");
                var request = new UpdateStopStatusRequest
                {
                    RouteId = routeId.ToString(),
                    StopId = stopId.ToString()
                };
                return await _client.UpdateStopStatusAsync(request);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                _logger.LogWarning($"Route or stop not found: Route ID={routeId}, Stop ID={stopId}");
                return null;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogWarning($"Invalid ID format: Route ID={routeId}, Stop ID={stopId}");
                throw;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.FailedPrecondition)
            {
                _logger.LogWarning($"Cannot update stop status: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating stop status for stop {stopId} in route {routeId}");
                throw;
            }
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
}