using AutoMapper;
using Grpc.Core;
using RouteService.Application.Services;

namespace RouteService.API.Services
{
    public class GrpcRouteService : RouteService.RouteServiceBase
    {
        private readonly IRouteService _routeService;
        private readonly IMapper _mapper;
        private readonly ILogger<GrpcRouteService> _logger;

        public GrpcRouteService(IRouteService routeService, ILogger<GrpcRouteService> logger, IMapper mapper)
        {
            _routeService = routeService;
            _logger = logger;
            _mapper = mapper;
        }

        public override async Task<RouteResponse> GetRoute(GetRouteRequest request, ServerCallContext context)
        {
            try
            {
                var routeId = Guid.Parse(request.RouteId);
                var route = await _routeService.GetRouteAsync(routeId);

                if (route == null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, $"Route with ID {request.RouteId} not found"));
                }

                return _mapper.Map<RouteResponse>(route);
            }
            catch (FormatException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid route ID format"));
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving route");
                throw new RpcException(new Status(StatusCode.Internal, "Internal error retrieving route"));
            }
        }

        public override async Task<ListPaginatedRoutesResponse> GetAllRoutes(GetAllRoutesRequest request, ServerCallContext context)
        {
            try
            {
                var routes = await _routeService.ListRoutesAsync(request.PageSize, request.PageNumber, request.Filter, (Domain.Enums.RouteStatus)request.Status);
                var response = new ListPaginatedRoutesResponse
                {
                    PageCount = routes.PageCount,
                    TotalCount = routes.TotalCount
                };
                response.Routes.AddRange(_mapper.Map<List<RouteMessage>>(routes));
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all routes");
                throw new RpcException(new Status(StatusCode.Internal, "Internal error retrieving routes"));
            }
        }

        public override async Task<RoutesResponse> GetRoutesByVehicle(GetRoutesByVehicleRequest request, ServerCallContext context)
        {
            try
            {
                var vehicleId = Guid.Parse(request.VehicleId);
                var routes = await _routeService.GetRoutesByVehicleIdAsync(vehicleId);

                var response = new RoutesResponse();
                response.Routes.AddRange(_mapper.Map<List<RouteMessage>>(routes));

                return response;
            }
            catch (FormatException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid vehicle ID format"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving routes by vehicle");
                throw new RpcException(new Status(StatusCode.Internal, "Internal error retrieving routes"));
            }
        }

        public override async Task<RoutesResponse> GetRoutesByDriver(GetRoutesByDriverRequest request, ServerCallContext context)
        {
            try
            {
                var driverId = Guid.Parse(request.DriverId);
                var routes = await _routeService.GetRoutesByDriverIdAsync(driverId);

                var response = new RoutesResponse();
                response.Routes.AddRange(_mapper.Map<List<RouteMessage>>(routes));

                return response;
            }
            catch (FormatException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid driver ID format"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving routes by driver");
                throw new RpcException(new Status(StatusCode.Internal, "Internal error retrieving routes"));
            }
        }

        //public override async Task<RoutesResponse> GetRoutesByStatus(GetRoutesByStatusRequest request, ServerCallContext context)
        //{
        //    try
        //    {
        //        var routes = await _routeService.GetRoutesByStatusAsync(request.Status);

        //        var response = new RoutesResponse();
        //        response.Routes.AddRange(_mapper.Map<List<RouteMessage>>(routes));

        //        return response;
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error retrieving routes by status");
        //        throw new RpcException(new Status(StatusCode.Internal, "Internal error retrieving routes"));
        //    }
        //}

        public override async Task<RoutesResponse> GetRoutesByDateRange(GetRoutesByDateRangeRequest request, ServerCallContext context)
        {
            try
            {
                var startDate = DateTime.Parse(request.StartDate);
                var endDate = DateTime.Parse(request.EndDate);

                var routes = await _routeService.GetRoutesByDateRangeAsync(startDate, endDate);

                var response = new RoutesResponse();
                response.Routes.AddRange(_mapper.Map<List<RouteMessage>>(routes));


                return response;
            }
            catch (FormatException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid date format"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving routes by date range");
                throw new RpcException(new Status(StatusCode.Internal, "Internal error retrieving routes"));
            }
        }

        //public override async Task<RouteResponse> CreateRoute(CreateRouteRequest request, ServerCallContext context)
        //{
        //    try
        //    {
        //        var vehicleId = Guid.Parse(request.VehicleId);
        //        var driverId = Guid.Parse(request.DriverId);
        //        var startTime = DateTime.Parse(request.StartTime);

        //        var stopDtos = request.Stops.Select(MapToRouteStopDto).ToList();

        //        var route = await _routeService.CreateRouteAsync(
        //            request.Name,
        //            vehicleId,
        //            driverId,
        //            startTime,
        //            stopDtos
        //        );

        //        return _mapper.Map<RouteResponse>(route);
        //    }
        //    catch (FormatException)
        //    {
        //        throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid ID or date format"));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error creating route");
        //        throw new RpcException(new Status(StatusCode.Internal, "Internal error creating route"));
        //    }
        //}

        public override async Task<RouteResponse> UpdateRoute(UpdateRouteRequest request, ServerCallContext context)
        {
            try
            {
                var routeId = Guid.Parse(request.RouteId);
                var vehicleId = Guid.Parse(request.VehicleId);
                var driverId = Guid.Parse(request.DriverId);
                var startTime = DateTime.Parse(request.StartTime);

                var route = await _routeService.UpdateRouteAsync(
                    routeId,
                    request.Name,
                    vehicleId,
                    driverId,
                    startTime
                );

                return _mapper.Map<RouteResponse>(route);
            }
            catch (FormatException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid ID or date format"));
            }
            catch (KeyNotFoundException ex)
            {
                throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating route");
                throw new RpcException(new Status(StatusCode.Internal, "Internal error updating route"));
            }
        }

        public override async Task<DeleteRouteResponse> DeleteRoute(DeleteRouteRequest request, ServerCallContext context)
        {
            try
            {
                var routeId = Guid.Parse(request.RouteId);
                await _routeService.DeleteRouteAsync(routeId);

                return new DeleteRouteResponse
                {
                    Success = true,
                    Message = $"Route {request.RouteId} deleted successfully"
                };
            }
            catch (FormatException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid route ID format"));
            }
            catch (KeyNotFoundException ex)
            {
                throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting route");
                throw new RpcException(new Status(StatusCode.Internal, "Internal error deleting route"));
            }
        }

        public override async Task<RouteResponse> OptimizeRoute(OptimizeRouteRequest request, ServerCallContext context)
        {
            try
            {
                var routeId = Guid.Parse(request.RouteId);
                var route = await _routeService.OptimizeRouteAsync(routeId);

                return _mapper.Map<RouteResponse>(route);
            }
            catch (FormatException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid route ID format"));
            }
            catch (KeyNotFoundException ex)
            {
                throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error optimizing route");
                throw new RpcException(new Status(StatusCode.Internal, "Internal error optimizing route"));
            }
        }

        public override async Task<RouteResponse> StartRoute(StartRouteRequest request, ServerCallContext context)
        {
            try
            {
                var routeId = Guid.Parse(request.RouteId);
                var route = await _routeService.StartRouteAsync(routeId);

                return _mapper.Map<RouteResponse>(route);
            }
            catch (FormatException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid route ID format"));
            }
            catch (KeyNotFoundException ex)
            {
                throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                throw new RpcException(new Status(StatusCode.FailedPrecondition, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting route");
                throw new RpcException(new Status(StatusCode.Internal, "Internal error starting route"));
            }
        }

        public override async Task<RouteResponse> CompleteRoute(CompleteRouteRequest request, ServerCallContext context)
        {
            try
            {
                var routeId = Guid.Parse(request.RouteId);
                var route = await _routeService.CompleteRouteAsync(routeId);

                return _mapper.Map<RouteResponse>(route);
            }
            catch (FormatException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid route ID format"));
            }
            catch (KeyNotFoundException ex)
            {
                throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                throw new RpcException(new Status(StatusCode.FailedPrecondition, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing route");
                throw new RpcException(new Status(StatusCode.Internal, "Internal error completing route"));
            }
        }

        public override async Task<RouteResponse> CancelRoute(CancelRouteRequest request, ServerCallContext context)
        {
            try
            {
                var routeId = Guid.Parse(request.RouteId);
                var route = await _routeService.CancelRouteAsync(routeId, request.Reason);

                return _mapper.Map<RouteResponse>(route);
            }
            catch (FormatException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid route ID format"));
            }
            catch (KeyNotFoundException ex)
            {
                throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                throw new RpcException(new Status(StatusCode.FailedPrecondition, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling route");
                throw new RpcException(new Status(StatusCode.Internal, "Internal error cancelling route"));
            }
        }

        //public override async Task<RouteResponse> AddStopToRoute(AddStopToRouteRequest request, ServerCallContext context)
        //{
        //    try
        //    {
        //        var routeId = Guid.Parse(request.RouteId);
        //        var stopDto = MapToRouteStopDto(request.Stop);

        //        var route = await _routeService.AddStopToRouteAsync(routeId, stopDto);

        //        return _mapper.Map<RouteResponse>(route);

        //    }
        //    catch (FormatException)
        //    {
        //        throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid ID or date format"));
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        throw new RpcException(new Status(StatusCode.FailedPrecondition, ex.Message));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error adding stop to route");
        //        throw new RpcException(new Status(StatusCode.Internal, "Internal error adding stop to route"));
        //    }
        //}

        //public override async Task<RouteResponse> UpdateStopStatus(UpdateStopStatusRequest request, ServerCallContext context)
        //{
        //    try
        //    {
        //        var routeId = Guid.Parse(request.RouteId);
        //        var stopId = Guid.Parse(request.StopId);

        //        var route = await _routeService.UpdateStopStatusAsync(routeId, stopId, request.Status);

        //        return _mapper.Map<RouteResponse>(route);
        //    }
        //    catch (FormatException)
        //    {
        //        throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid ID format"));
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        throw new RpcException(new Status(StatusCode.FailedPrecondition, ex.Message));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error updating stop status");
        //        throw new RpcException(new Status(StatusCode.Internal, "Internal error updating stop status"));
        //    }
        //}

        //#region Helper Methods

        //private RouteMessage MapToRouteMessage(RouteDto routeDto)
        //{
        //    if (routeDto == null)
        //        return null;

        //    var routeMessage = new RouteMessage
        //    {
        //        Id = routeDto.Id.ToString(),
        //        Name = routeDto.Name,
        //        VehicleId = routeDto.VehicleId.ToString(),
        //        DriverId = routeDto.DriverId.ToString(),
        //        StartTime = routeDto.StartTime.ToString("o"), // ISO 8601 format
        //        Status = routeDto.Status,
        //        CreatedAt = routeDto.CreatedAt.ToString("o"),
        //        TotalDistance = routeDto.TotalDistance,
        //        EstimatedDurationMinutes = (int)routeDto.EstimatedDuration.TotalMinutes
        //    };

        //    if (routeDto.EndTime.HasValue)
        //        routeMessage.EndTime = routeDto.EndTime.Value.ToString("o");

        //    if (routeDto.UpdatedAt.HasValue)
        //        routeMessage.UpdatedAt = routeDto.UpdatedAt.Value.ToString("o");

        //    if (routeDto.Stops != null)
        //    {
        //        foreach (var stop in routeDto.Stops)
        //        {
        //            routeMessage.Stops.Add(MapToRouteStopMessage(stop));
        //        }
        //    }

        //    return routeMessage;
        //}

        //private RouteStopMessage MapToRouteStopMessage(RouteStopDto stopDto)
        //{
        //    if (stopDto == null)
        //        return null;

        //    var stopMessage = new RouteStopMessage
        //    {
        //        Id = stopDto.Id.ToString(),
        //        SequenceNumber = stopDto.SequenceNumber,
        //        Name = stopDto.Name,
        //        Address = stopDto.Address,
        //        Latitude = stopDto.Latitude,
        //        Longitude = stopDto.Longitude,
        //        PlannedArrivalTime = stopDto.PlannedArrivalTime.ToString("o"),
        //        Status = stopDto.Status,
        //        EstimatedDurationMinutes = stopDto.EstimatedDurationMinutes
        //    };

        //    if (stopDto.ActualArrivalTime.HasValue)
        //        stopMessage.ActualArrivalTime = stopDto.ActualArrivalTime.Value.ToString("o");

        //    if (stopDto.DepartureTime.HasValue)
        //        stopMessage.DepartureTime = stopDto.DepartureTime.Value.ToString("o");

        //    if (!string.IsNullOrEmpty(stopDto.Notes))
        //        stopMessage.Notes = stopDto.Notes;

        //    return stopMessage;
        //}

        //private RouteStopDto MapToRouteStopDto(RouteStopMessage stopMessage)
        //{
        //    if (stopMessage == null)
        //        return null;

        //    var stopDto = new RouteStopDto
        //    {
        //        SequenceNumber = stopMessage.SequenceNumber,
        //        Name = stopMessage.Name,
        //        Address = stopMessage.Address,
        //        Latitude = stopMessage.Latitude,
        //        Longitude = stopMessage.Longitude,
        //        PlannedArrivalTime = DateTime.Parse(stopMessage.PlannedArrivalTime),
        //        EstimatedDurationMinutes = stopMessage.EstimatedDurationMinutes,
        //        Notes = stopMessage.Notes
        //    };

        //    if (!string.IsNullOrEmpty(stopMessage.Id))
        //        stopDto.Id = Guid.Parse(stopMessage.Id);
        //    else
        //        stopDto.Id = Guid.NewGuid();

        //    if (!string.IsNullOrEmpty(stopMessage.ActualArrivalTime))
        //        stopDto.ActualArrivalTime = DateTime.Parse(stopMessage.ActualArrivalTime);

        //    if (!string.IsNullOrEmpty(stopMessage.DepartureTime))
        //        stopDto.DepartureTime = DateTime.Parse(stopMessage.DepartureTime);

        //    return stopDto;
        //}

    }
}
