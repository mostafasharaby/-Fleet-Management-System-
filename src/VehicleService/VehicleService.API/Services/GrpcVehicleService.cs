using AutoMapper;
using Grpc.Core;
using VehicleService.Application.Services;
using VehicleService.Server;

namespace VehicleService.API.Services
{
    public class GrpcVehicleService : VehicleService.Server.VehicleService.VehicleServiceBase
    {
        private readonly IVehicleService _vehicleService;
        private readonly ILogger<GrpcVehicleService> _logger;
        private readonly IMapper _mapper;
        public GrpcVehicleService(IVehicleService vehicleService, ILogger<GrpcVehicleService> logger, IMapper mapper)
        {
            _vehicleService = vehicleService;
            _logger = logger;
            _mapper = mapper;
        }

        public override async Task<VehicleResponse> GetVehicle(GetVehicleRequest request, ServerCallContext context)
        {
            var vehicleId = Guid.Parse(request.VehicleId);
            var vehicle = await _vehicleService.GetVehicleAsync(vehicleId);
            if (vehicle == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Vehicle not found"));
            }

            return _mapper.Map<VehicleResponse>(vehicle);
        }
        public override async Task<ListVehiclesResponse> ListVehicles(ListVehiclesRequest request, ServerCallContext context)
        {
            var mappedStatus = request.Status == VehicleStatus.OutOfService ? null : (Domain.Enums.VehicleStatus?)request.Status;
            var (vehicles, totalCount, pageCount) = await _vehicleService.ListVehiclesAsync(request.PageSize, request.PageNumber, request.Filter, mappedStatus);
            var response = new ListVehiclesResponse
            {
                TotalCount = totalCount,
                PageCount = pageCount
            };
            response.Vehicles.AddRange(_mapper.Map<List<VehicleResponse>>(vehicles));
            return response;
        }

        public override async Task<VehicleResponse> CreateVehicle(CreateVehicleRequest request, ServerCallContext context)
        {
            var vehicle = await _vehicleService.CreateVehicleAsync(
                request.RegistrationNumber,
                request.Model,
                request.Manufacturer,
                request.Year,
                request.Vin,
                (Domain.Enums.VehicleType)request.Type,
                request.FuelCapacity,
                request.CurrentFuelLevel,
                request.OdometerReading);

            var response = _mapper.Map<VehicleResponse>(vehicle);
            return response;
        }

        public override async Task<VehicleResponse> UpdateVehicle(UpdateVehicleRequest request, ServerCallContext context)
        {
            var vehicleId = Guid.Parse(request.VehicleId);
            var vehicle = await _vehicleService.UpdateVehicleAsync(
                vehicleId,
                request.RegistrationNumber,
                request.Model,
                request.Manufacturer,
                request.Year,
                request.Vin,
                (Domain.Enums.VehicleType)request.Type,
                request.FuelCapacity,
                request.CurrentFuelLevel,
                request.OdometerReading,
                (Domain.Enums.VehicleStatus)request.Status);

            if (vehicle == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Vehicle with ID {vehicleId} not found"));
            }

            var response = _mapper.Map<VehicleResponse>(vehicle);
            return response;
        }

        public override async Task<DeleteVehicleResponse> DeleteVehicle(DeleteVehicleRequest request, ServerCallContext context)
        {
            var vehicleId = Guid.Parse(request.VehicleId);
            var deleted = await _vehicleService.DeleteVehicleAsync(vehicleId);
            if (!deleted)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Vehicle with ID {vehicleId} not found"));
            }
            var response = new DeleteVehicleResponse
            {
                Success = true,
                Message = $"Vehicle with ID {vehicleId} deleted successfully"
            };
            return response;
        }
        public override async Task<VehicleResponse> AssignVehicleToDriver(AssignVehicleRequest request, ServerCallContext context)
        {
            var vehicleId = Guid.Parse(request.VehicleId);
            var driverId = Guid.Parse(request.DriverId);
            var assignmentStart = DateTimeOffset.FromUnixTimeSeconds(request.AssignmentStartTimestamp).UtcDateTime;
            DateTime? assignmentEnd = request.AssignmentEndTimestamp > 0 ? DateTimeOffset.FromUnixTimeSeconds(request.AssignmentEndTimestamp).UtcDateTime : (DateTime?)null;

            var vehicle = await _vehicleService.AssignVehicleToDriverAsync(vehicleId, driverId, assignmentStart);
            if (vehicle == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Vehicle with ID {vehicleId} not found"));
            }
            var response = _mapper.Map<VehicleResponse>(vehicle);
            return response;
        }

        public override async Task StreamVehicleLocation(VehicleLocationStreamRequest request, IServerStreamWriter<VehicleLocationResponse> responseStream, ServerCallContext context)
        {
            var vehicleId = Guid.Parse(request.VehicleId);
            while (!context.CancellationToken.IsCancellationRequested)
            {
                try
                {
                    var vehicle = await _vehicleService.GetVehicleAsync(vehicleId);
                    if (vehicle == null)
                    {
                        throw new RpcException(new Status(StatusCode.NotFound, $"Vehicle with ID {vehicleId} not found"));
                    }

                    var vehicleLocationResponse = new VehicleLocationResponse
                    {
                        VehicleId = request.VehicleId,
                        Latitude = 37.7749,
                        Longitude = -122.4194,
                        Speed = 0,
                        Heading = 0,
                        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                    };
                    await responseStream.WriteAsync(vehicleLocationResponse);
                    await Task.Delay(1000, context.CancellationToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while streaming vehicle location");
                    break;
                }
            }
        }
    }
}
