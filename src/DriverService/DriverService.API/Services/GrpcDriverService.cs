using AutoMapper;
using DriverService.Application.Services;
using DriverService.Domain.Models;
using Grpc.Core;

namespace DriverService.API.Services
{
    public class GrpcDriverService : DriverService.DriverServiceBase
    {
        private readonly IDriverService _driverService;
        private readonly ILogger<GrpcDriverService> _logger;
        private readonly IMapper _mapper;
        public GrpcDriverService(IDriverService driverService, ILogger<GrpcDriverService> logger, IMapper mapper)
        {
            _driverService = driverService;
            _logger = logger;
            _mapper = mapper;
        }

        public override async Task<DriverResponse> GetDriver(GetDriverRequest request, ServerCallContext context)
        {
            try
            {
                var driverId = Guid.Parse(request.DriverId);
                var driver = await _driverService.GetDriverAsync(driverId);

                if (driver == null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, $"Driver with ID {request.DriverId} not found"));
                }

                return _mapper.Map<DriverResponse>(driver);
            }
            catch (FormatException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid driver ID format"));
            }
            catch (Exception ex) when (!(ex is RpcException))
            {
                _logger.LogError(ex, $"Error retrieving driver {request.DriverId}");
                throw new RpcException(new Status(StatusCode.Internal, "An internal error occurred"));
            }
        }

        public override async Task<ListDriversResponse> ListDrivers(ListDriversRequest request, ServerCallContext context)
        {
            try
            {
                var pageSize = request.PageSize > 0 ? request.PageSize : 10;
                var pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;
                Domain.Enums.DriverStatus? status = request.Status != DriverStatus.StatusUnknown
                     ? (Domain.Enums.DriverStatus)request.Status
                     : null;

                var (drivers, totalCount, pageCount) = await _driverService.ListDriversAsync(
                    pageSize,
                    pageNumber,
                    request.Filter,
                    status);

                var response = new ListDriversResponse
                {
                    TotalCount = totalCount,
                    PageCount = pageCount
                };

                foreach (var driver in drivers)
                {
                    response.Drivers.Add(_mapper.Map<DriverResponse>(driver));

                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing drivers");
                throw new RpcException(new Status(StatusCode.Internal, "An internal error occurred"));
            }
        }

        public override async Task<DriverResponse> CreateDriver(CreateDriverRequest request, ServerCallContext context)
        {
            try
            {
                var licenseExpiry = DateTimeOffset.FromUnixTimeSeconds(request.LicenseExpiry).UtcDateTime;
                var driverType = (Domain.Enums.DriverType)request.Type;

                var driver = await _driverService.CreateDriverAsync(
                    request.FirstName,
                    request.LastName,
                    request.Email,
                    request.PhoneNumber,
                    request.LicenseNumber,
                    request.LicenseState,
                    licenseExpiry,
                    driverType);

                return _mapper.Map<DriverResponse>(driver);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating driver");
                throw new RpcException(new Status(StatusCode.Internal, "An internal error occurred"));
            }
        }

        public override async Task<DriverResponse> UpdateDriver(UpdateDriverRequest request, ServerCallContext context)
        {
            try
            {
                var driverId = Guid.Parse(request.DriverId);
                var licenseExpiry = DateTimeOffset.FromUnixTimeSeconds(request.LicenseExpiry).UtcDateTime;
                var driverStatus = (Domain.Enums.DriverStatus)request.Status;
                var driverType = (Domain.Enums.DriverType)request.Type;

                var driver = await _driverService.UpdateDriverAsync(
                    driverId,
                    request.FirstName,
                    request.LastName,
                    request.Email,
                    request.PhoneNumber,
                    request.LicenseNumber,
                    request.LicenseState,
                    licenseExpiry,
                    driverStatus,
                    driverType);

                if (driver == null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, $"Driver with ID {request.DriverId} not found"));
                }

                return _mapper.Map<DriverResponse>(driver);

            }
            catch (FormatException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid driver ID format"));
            }
            catch (Exception ex) when (!(ex is RpcException))
            {
                _logger.LogError(ex, $"Error updating driver {request.DriverId}");
                throw new RpcException(new Status(StatusCode.Internal, "An internal error occurred"));
            }
        }

        public override async Task<DeleteDriverResponse> DeleteDriver(DeleteDriverRequest request, ServerCallContext context)
        {
            try
            {
                var driverId = Guid.Parse(request.DriverId);
                var result = await _driverService.DeleteDriverAsync(driverId);

                if (!result)
                {
                    return new DeleteDriverResponse
                    {
                        Success = false,
                        Message = $"Driver with ID {request.DriverId} not found"
                    };
                }

                return new DeleteDriverResponse
                {
                    Success = true,
                    Message = $"Driver with ID {request.DriverId} successfully deleted"
                };
            }
            catch (FormatException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid driver ID format"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting driver {request.DriverId}");
                throw new RpcException(new Status(StatusCode.Internal, "An internal error occurred"));
            }
        }

        public override async Task<GetDriverAssignmentsResponse> GetDriverAssignments(GetDriverAssignmentsRequest request, ServerCallContext context)
        {
            try
            {
                var driverId = Guid.Parse(request.DriverId);
                var fromDate = DateTimeOffset.FromUnixTimeSeconds(request.FromDate).UtcDateTime;
                var toDate = DateTimeOffset.FromUnixTimeSeconds(request.ToDate).UtcDateTime;

                var assignments = await _driverService.GetDriverAssignmentsAsync(driverId, fromDate, toDate);

                var response = new GetDriverAssignmentsResponse();
                foreach (var assignment in assignments)
                {
                    response.Assignments.Add(_mapper.Map<AssignmentResponse>(assignment));
                }

                return response;
            }
            catch (FormatException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid driver ID format"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving assignments for driver {request.DriverId}");
                throw new RpcException(new Status(StatusCode.Internal, "An internal error occurred"));
            }
        }

        public override async Task<AssignmentResponse> AssignVehicle(AssignVehicleRequest request, ServerCallContext context)
        {
            try
            {
                var driverId = Guid.Parse(request.DriverId);
                var vehicleId = Guid.Parse(request.VehicleId);
                var startTime = DateTimeOffset.FromUnixTimeSeconds(request.StartTime).UtcDateTime;
                var endTime = DateTimeOffset.FromUnixTimeSeconds(request.EndTime).UtcDateTime;

                var assignment = await _driverService.AssignVehicleToDriverAsync(
                    driverId,
                    vehicleId,
                    startTime,
                    endTime,
                    request.Notes);

                if (assignment == null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, $"Driver with ID {request.DriverId} not found"));
                }

                return _mapper.Map<AssignmentResponse>(assignment);
            }
            catch (FormatException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid ID format"));
            }
            catch (InvalidOperationException ex)
            {
                throw new RpcException(new Status(StatusCode.FailedPrecondition, ex.Message));
            }
            catch (Exception ex) when (!(ex is RpcException))
            {
                _logger.LogError(ex, $"Error assigning vehicle {request.VehicleId} to driver {request.DriverId}");
                throw new RpcException(new Status(StatusCode.Internal, "An internal error occurred"));
            }
        }

        public override async Task<AssignmentResponse> CompleteAssignment(CompleteAssignmentRequest request, ServerCallContext context)
        {
            try
            {
                var assignmentId = Guid.Parse(request.AssignmentId);

                var assignment = await _driverService.CompleteAssignmentAsync(
                    assignmentId,
                    request.FinalOdometer,
                    request.FuelLevel,
                    request.Notes);

                if (assignment == null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, $"Assignment with ID {request.AssignmentId} not found"));
                }

                return _mapper.Map<AssignmentResponse>(assignment);
            }
            catch (FormatException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid assignment ID format"));
            }
            catch (InvalidOperationException ex)
            {
                throw new RpcException(new Status(StatusCode.FailedPrecondition, ex.Message));
            }
            catch (Exception ex) when (!(ex is RpcException))
            {
                _logger.LogError(ex, $"Error completing assignment {request.AssignmentId}");
                throw new RpcException(new Status(StatusCode.Internal, "An internal error occurred"));
            }
        }

        public override async Task<GetDriverAvailabilityResponse> GetDriverAvailability(GetDriverAvailabilityRequest request, ServerCallContext context)
        {
            try
            {
                var driverId = Guid.Parse(request.DriverId);
                var fromDate = DateTimeOffset.FromUnixTimeSeconds(request.FromDate).UtcDateTime;
                var toDate = DateTimeOffset.FromUnixTimeSeconds(request.ToDate).UtcDateTime;

                var availabilitySlots = await _driverService.GetDriverAvailabilityAsync(driverId, fromDate, toDate);

                var response = new GetDriverAvailabilityResponse();
                foreach (var slot in availabilitySlots)
                {
                    response.AvailabilitySlots.Add(new AvailabilitySlot
                    {
                        StartTime = ((DateTimeOffset)slot.Start.ToUniversalTime()).ToUnixTimeSeconds(),
                        EndTime = ((DateTimeOffset)slot.End.ToUniversalTime()).ToUnixTimeSeconds(),
                        IsAvailable = slot.IsAvailable,
                        AssignmentId = slot.AssignmentId?.ToString() ?? ""
                    });
                }

                return response;
            }
            catch (FormatException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid driver ID format"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving availability for driver {request.DriverId}");
                throw new RpcException(new Status(StatusCode.Internal, "An internal error occurred"));
            }
        }

        public override async Task<ScheduleResponse> ScheduleDriver(ScheduleDriverRequest request, ServerCallContext context)
        {
            try
            {
                var driverId = Guid.Parse(request.DriverId);
                var scheduleEntries = new List<ScheduleEntry>();

                foreach (var slot in request.ScheduleSlots)
                {
                    scheduleEntries.Add(new ScheduleEntry
                    {
                        DriverId = driverId,
                        StartTime = DateTimeOffset.FromUnixTimeSeconds(slot.StartTime).UtcDateTime,
                        EndTime = DateTimeOffset.FromUnixTimeSeconds(slot.EndTime).UtcDateTime,
                        Type = (Domain.Enums.ScheduleType)slot.Type,
                        Notes = slot.Notes
                    });
                }

                var (entries, conflicts) = await _driverService.ScheduleDriverAsync(driverId, scheduleEntries);

                var response = new ScheduleResponse
                {
                    Success = conflicts.Count == 0,
                    Message = conflicts.Count == 0
                        ? $"Successfully scheduled {entries.Count} entries"
                        : $"Scheduled {entries.Count} entries with {conflicts.Count} conflicts"
                };

                foreach (var conflict in conflicts)
                {
                    response.Conflicts.Add(new ScheduleConflict
                    {
                        StartTime = ((DateTimeOffset)conflict.StartTime.ToUniversalTime()).ToUnixTimeSeconds(),
                        EndTime = ((DateTimeOffset)conflict.EndTime.ToUniversalTime()).ToUnixTimeSeconds(),
                        Reason = conflict.Reason
                    });
                }

                return response;
            }
            catch (FormatException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid driver ID format"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error scheduling driver {request.DriverId}");
                throw new RpcException(new Status(StatusCode.Internal, "An internal error occurred"));
            }
        }

    }
}
