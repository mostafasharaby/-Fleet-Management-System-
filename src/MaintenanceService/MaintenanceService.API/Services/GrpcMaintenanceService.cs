using AutoMapper;
using Grpc.Core;
using MaintenanceService.API.Protos;
using MaintenanceService.Application.Services;

namespace MaintenanceService.API.Services
{
    public class GrpcMaintenanceService : Protos.MaintenanceService.MaintenanceServiceBase
    {
        private readonly IMaintenanceService _maintenanceService;
        private readonly IMapper _mapper;
        private readonly ILogger<GrpcMaintenanceService> _logger;

        public GrpcMaintenanceService(
            IMaintenanceService maintenanceService,
            IMapper mapper,
            ILogger<GrpcMaintenanceService> logger)
        {
            _maintenanceService = maintenanceService;
            _mapper = mapper;
            _logger = logger;
        }

        public override async Task<MaintenanceScheduleResponse> GetMaintenanceSchedule(
            GetMaintenanceScheduleRequest request, ServerCallContext context)
        {
            try
            {
                var vehicleId = Guid.Parse(request.VehicleId);
                var tasks = await _maintenanceService.GetScheduledTasksAsync(vehicleId, request.IncludeCompleted);
                var protoTasks = _mapper.Map<List<MaintenanceTask>>(tasks);
                foreach (var task in protoTasks)
                {
                    _logger.LogInformation("Mapped task: {Description}, Parts: {Parts}",
                        task.TaskDescription,
                        string.Join(", ", task.RequiredParts.Select(p => $"{p.PartId}:{p.PartName}:{p.Quantity}")));
                }
                var response = new MaintenanceScheduleResponse
                {
                    ScheduledTasks = { protoTasks } // there is issue in mapping RequiredParts ---> will be solved later as i d.k the reason 
                };
                return response;
            }
            catch (FormatException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid vehicle ID format"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving maintenance schedule");
                throw new RpcException(new Status(StatusCode.Internal, "Internal error retrieving maintenance schedule"));
            }
        }

        public override async Task<MaintenanceEventResponse> RecordMaintenanceEvent(
            MaintenanceEventRequest request, ServerCallContext context)
        {
            try
            {
                var maintenanceEvent = _mapper.Map<Domain.Models.MaintenanceEvent>(request.Event);
                var recordedEvent = await _maintenanceService.RecordMaintenanceEventAsync(maintenanceEvent);
                return new MaintenanceEventResponse
                {
                    Success = true,
                    EventId = recordedEvent.Id.ToString(),
                    Message = "Maintenance event recorded successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording maintenance event");
                throw new RpcException(new Status(StatusCode.Internal, "Internal error recording maintenance event"));
            }
        }

        public override async Task<ScheduleMaintenanceResponse> ScheduleMaintenance(
            ScheduleMaintenanceRequest request, ServerCallContext context)
        {
            try
            {
                var task = _mapper.Map<Domain.Models.MaintenanceTask>(request.Task);
                var scheduledTask = await _maintenanceService.ScheduleMaintenanceAsync(task);
                return new ScheduleMaintenanceResponse
                {
                    Success = true,
                    TaskId = scheduledTask.Id.ToString(),
                    Message = "Maintenance task scheduled successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scheduling maintenance");
                throw new RpcException(new Status(StatusCode.Internal, "Internal error scheduling maintenance"));
            }
        }

        public override async Task<UpdateMaintenanceStatusResponse> UpdateMaintenanceStatus(
            UpdateMaintenanceStatusRequest request, ServerCallContext context)
        {
            try
            {
                var taskId = Guid.Parse(request.TaskId);
                await _maintenanceService.UpdateMaintenanceStatusAsync(
                    taskId,
                    (Domain.Enums.MaintenanceStatus)request.NewStatus,
                    request.UpdatedBy,
                    request.Notes);
                return new UpdateMaintenanceStatusResponse
                {
                    Success = true,
                    Message = "Maintenance status updated successfully"
                };
            }
            catch (FormatException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid task ID format"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating maintenance status");
                throw new RpcException(new Status(StatusCode.Internal, "Internal error updating maintenance status"));
            }
        }

        public override async Task<GetMaintenanceHistoryResponse> GetMaintenanceHistory(
            GetMaintenanceHistoryRequest request, ServerCallContext context)
        {
            try
            {
                var vehicleId = Guid.Parse(request.VehicleId);
                var startDate = DateTimeOffset.FromUnixTimeSeconds((long)request.StartDate).DateTime;
                var endDate = DateTimeOffset.FromUnixTimeSeconds((long)request.EndDate).DateTime;

                var (events, totalCount, totalPages) = await _maintenanceService.GetMaintenanceHistoryAsync(
                    vehicleId,
                    startDate,
                    endDate,
                    request.PageSize,
                    request.PageNumber);

                var response = new GetMaintenanceHistoryResponse
                {
                    Events = { _mapper.Map<MaintenanceEvent[]>(events) },
                    TotalCount = totalCount,
                    PageNumber = request.PageNumber,
                    TotalPages = totalPages
                };
                return response;
            }
            catch (FormatException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid vehicle ID format"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving maintenance history");
                throw new RpcException(new Status(StatusCode.Internal, "Internal error retrieving maintenance history"));
            }
        }

        public override async Task<VehicleHealthMetricsResponse> GetVehicleHealthMetrics(
            VehicleHealthMetricsRequest request, ServerCallContext context)
        {
            try
            {
                var vehicleId = Guid.Parse(request.VehicleId);
                var metrics = await _maintenanceService.GetVehicleHealthMetricsAsync(vehicleId);
                return _mapper.Map<VehicleHealthMetricsResponse>(metrics);
            }
            catch (FormatException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid vehicle ID format"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving vehicle health metrics");
                throw new RpcException(new Status(StatusCode.Internal, "Internal error retrieving vehicle health metrics"));
            }
        }
    }
}
