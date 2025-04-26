using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using TelemetryService.API.Protos;
using TelemetryService.Application.DTOs;
using TelemetryService.Application.Services;

namespace TelemetryService.API.Services
{
    public class TelemetryGrpcService : Protos.TelemetryService.TelemetryServiceBase
    {
        private readonly ILogger<TelemetryGrpcService> _logger;
        private readonly ITelemetryService _telemetryService;
        private readonly IAlertThresholdService _alertThresholdService;
        private readonly IMapper _mapper;

        public TelemetryGrpcService(
            ILogger<TelemetryGrpcService> logger,
            ITelemetryService telemetryService,
            IAlertThresholdService alertThresholdService,
            IMapper mapper)
        {
            _logger = logger;
            _telemetryService = telemetryService;
            _alertThresholdService = alertThresholdService;
            _mapper = mapper;
        }

        public override async Task<TelemetryDataResponse> GetTelemetryDataById(
            GetTelemetryDataByIdRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Getting telemetry data with ID: {Id}", request.Id);

            try
            {
                var telemetryData = await _telemetryService.GetTelemetryDataByIdAsync(Guid.Parse(request.Id));
                if (telemetryData == null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, $"Telemetry data with ID {request.Id} not found"));
                }

                return MapToTelemetryDataResponse(telemetryData);
            }
            catch (FormatException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid ID format"));
            }
            catch (Exception ex) when (!(ex is RpcException))
            {
                _logger.LogError(ex, "Error getting telemetry data by ID");
                throw new RpcException(new Status(StatusCode.Internal, "Internal error occurred"));
            }
        }

        public override async Task<TelemetryDataListResponse> GetTelemetryDataByVehicleId(
            GetTelemetryDataByVehicleIdRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Getting telemetry data for vehicle ID: {VehicleId}, limit: {Limit}",
                request.VehicleId, request.Limit);

            try
            {
                var vehicleId = Guid.Parse(request.VehicleId);
                var limit = request.Limit > 0 ? request.Limit : 100;

                var telemetryDataList = await _telemetryService.GetTelemetryDataByVehicleIdAsync(vehicleId, limit);

                var response = new TelemetryDataListResponse();
                response.TelemetryData.AddRange(telemetryDataList.Select(MapToTelemetryDataResponse));

                return response;
            }
            catch (FormatException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid vehicle ID format"));
            }
            catch (Exception ex) when (!(ex is RpcException))
            {
                _logger.LogError(ex, "Error getting telemetry data by vehicle ID");
                throw new RpcException(new Status(StatusCode.Internal, "Internal error occurred"));
            }
        }

        public override async Task<TelemetryDataListResponse> GetTelemetryDataByTimeRange(
            GetTelemetryDataByTimeRangeRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Getting telemetry data for vehicle ID: {VehicleId} from {StartTime} to {EndTime}",
                request.VehicleId, request.StartTime, request.EndTime);

            try
            {
                var vehicleId = Guid.Parse(request.VehicleId);
                var startTime = request.StartTime.ToDateTime();
                var endTime = request.EndTime.ToDateTime();

                if (startTime > endTime)
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Start time must be before end time"));
                }

                var telemetryDataList = await _telemetryService.GetTelemetryDataByTimeRangeAsync(vehicleId, startTime, endTime);

                var response = new TelemetryDataListResponse();
                response.TelemetryData.AddRange(telemetryDataList.Select(MapToTelemetryDataResponse));

                return response;
            }
            catch (FormatException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid vehicle ID format"));
            }
            catch (Exception ex) when (!(ex is RpcException))
            {
                _logger.LogError(ex, "Error getting telemetry data by time range");
                throw new RpcException(new Status(StatusCode.Internal, "Internal error occurred"));
            }
        }

        public override async Task<TelemetryDataResponse> SendTelemetryData(
            TelemetryDataRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Received telemetry data for vehicle ID: {VehicleId}", request.VehicleId);

            try
            {
                var telemetryDataDto = new TelemetryDataDto
                {
                    VehicleId = Guid.Parse(request.VehicleId),
                    Timestamp = request.Timestamp.ToDateTime(),
                    Latitude = request.Latitude,
                    Longitude = request.Longitude,
                    Speed = request.Speed,
                    FuelLevel = request.FuelLevel,
                    EngineTemperature = request.EngineTemperature,
                    BatteryVoltage = request.BatteryVoltage,
                    EngineRpm = request.EngineRpm,
                    CheckEngineLightOn = request.CheckEngineLightOn,
                    OdometerReading = request.OdometerReading,
                    DiagnosticCode = request.DiagnosticCode
                };

                await _telemetryService.ProcessTelemetryDataAsync(telemetryDataDto);

                return MapToTelemetryDataResponse(telemetryDataDto);
            }
            catch (FormatException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid vehicle ID format"));
            }
            catch (Exception ex) when (!(ex is RpcException))
            {
                _logger.LogError(ex, "Error processing telemetry data");
                throw new RpcException(new Status(StatusCode.Internal, "Internal error occurred"));
            }
        }

        public override async Task<BatchTelemetryDataResponse> SendBatchTelemetryData(
            BatchTelemetryDataRequest request, ServerCallContext context)
        {
            var count = request.TelemetryData.Count;
            _logger.LogInformation("Received batch telemetry data with {Count} entries", count);

            try
            {
                var telemetryDataList = request.TelemetryData.Select(r => new TelemetryDataDto
                {
                    VehicleId = Guid.Parse(r.VehicleId),
                    Timestamp = r.Timestamp.ToDateTime(),
                    Latitude = r.Latitude,
                    Longitude = r.Longitude,
                    Speed = r.Speed,
                    FuelLevel = r.FuelLevel,
                    EngineTemperature = r.EngineTemperature,
                    BatteryVoltage = r.BatteryVoltage,
                    EngineRpm = r.EngineRpm,
                    CheckEngineLightOn = r.CheckEngineLightOn,
                    OdometerReading = r.OdometerReading,
                    DiagnosticCode = r.DiagnosticCode
                }).ToList();

                await _telemetryService.ProcessBatchTelemetryDataAsync(telemetryDataList);

                return new BatchTelemetryDataResponse
                {
                    ProcessedCount = count,
                    Success = true,
                    Message = $"Successfully processed {count} telemetry data entries"
                };
            }
            catch (FormatException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid vehicle ID format"));
            }
            catch (Exception ex) when (!(ex is RpcException))
            {
                _logger.LogError(ex, "Error processing batch telemetry data");
                throw new RpcException(new Status(StatusCode.Internal, "Internal error occurred"));
            }
        }

        public override async Task<TelemetryDataListResponse> GetLatestTelemetryForAllVehicles(
            GetLatestTelemetryRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Getting latest telemetry data for all vehicles");

            try
            {
                var telemetryDataList = await _telemetryService.GetLatestTelemetryForAllVehiclesAsync();

                var response = new TelemetryDataListResponse();
                response.TelemetryData.AddRange(telemetryDataList.Select(MapToTelemetryDataResponse));

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting latest telemetry data for all vehicles");
                throw new RpcException(new Status(StatusCode.Internal, "Internal error occurred"));
            }
        }

        public override async Task StreamVehicleTelemetry(
            StreamVehicleTelemetryRequest request,
            IServerStreamWriter<TelemetryDataResponse> responseStream,
            ServerCallContext context)
        {
            _logger.LogInformation("Starting telemetry stream for vehicle ID: {VehicleId}", request.VehicleId);

            try
            {
                var vehicleId = Guid.Parse(request.VehicleId);

                // Example implementation - in a real system, this would hook into a real-time data source
                // This is a simplified implementation for demonstration purposes

                // Get the latest data first
                var latestData = await _telemetryService.GetTelemetryDataByVehicleIdAsync(vehicleId, 1);

                if (latestData.Any())
                {
                    await responseStream.WriteAsync(MapToTelemetryDataResponse(latestData.First()));
                }

                // Simulate periodic updates until the client disconnects
                int counter = 0;
                while (!context.CancellationToken.IsCancellationRequested && counter < 100)
                {
                    await Task.Delay(1000, context.CancellationToken); // Simulate 1-second intervals

                    // In a real implementation, you'd get fresh data from a queue or subscription
                    // This is just a simulation
                    var simulatedData = SimulateNewTelemetryData(vehicleId);

                    await responseStream.WriteAsync(MapToTelemetryDataResponse(simulatedData));
                    counter++;
                }
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("Telemetry stream for vehicle {VehicleId} was canceled", request.VehicleId);
            }
            catch (FormatException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid vehicle ID format"));
            }
            catch (Exception ex) when (!(ex is RpcException))
            {
                _logger.LogError(ex, "Error streaming telemetry data");
                throw new RpcException(new Status(StatusCode.Internal, "Internal error occurred"));
            }
        }

        // Alert threshold operations
        public override async Task<AlertThresholdListResponse> GetAlertThresholdsByVehicleId(
            GetAlertThresholdsByVehicleIdRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Getting alert thresholds for vehicle ID: {VehicleId}", request.VehicleId);

            try
            {
                var vehicleId = Guid.Parse(request.VehicleId);
                var thresholds = await _alertThresholdService.GetAlertThresholdsByVehicleIdAsync(vehicleId);

                var response = new AlertThresholdListResponse();
                response.Thresholds.AddRange(thresholds.Select(MapToAlertThresholdResponse));

                return response;
            }
            catch (FormatException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid vehicle ID format"));
            }
            catch (Exception ex) when (!(ex is RpcException))
            {
                _logger.LogError(ex, "Error getting alert thresholds by vehicle ID");
                throw new RpcException(new Status(StatusCode.Internal, "Internal error occurred"));
            }
        }

        public override async Task<AlertThresholdResponse> GetAlertThresholdById(
            GetAlertThresholdByIdRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Getting alert threshold with ID: {Id}", request.Id);

            try
            {
                var thresholdId = Guid.Parse(request.Id);
                var threshold = await _alertThresholdService.GetAlertThresholdByIdAsync(thresholdId);

                if (threshold == null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, $"Alert threshold with ID {request.Id} not found"));
                }

                return MapToAlertThresholdResponse(threshold);
            }
            catch (FormatException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid ID format"));
            }
            catch (Exception ex) when (!(ex is RpcException))
            {
                _logger.LogError(ex, "Error getting alert threshold by ID");
                throw new RpcException(new Status(StatusCode.Internal, "Internal error occurred"));
            }
        }

        public override async Task<AlertThresholdResponse> CreateAlertThreshold(
            CreateAlertThresholdRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Creating alert threshold for vehicle ID: {VehicleId}, parameter: {ParameterName}",
                request.VehicleId, request.ParameterName);

            try
            {
                var alertThresholdDto = new AlertThresholdDto
                {
                    VehicleId = Guid.Parse(request.VehicleId),
                    ParameterName = request.ParameterName,
                    MinValue = request.MinValue,
                    MaxValue = request.MaxValue,
                    IsEnabled = request.IsEnabled,
                    AlertMessage = request.AlertMessage,
                    Severity = (Domain.Enums.AlertSeverity)request.Severity
                };

                var createdThreshold = await _alertThresholdService.CreateAlertThresholdAsync(alertThresholdDto);

                return MapToAlertThresholdResponse(createdThreshold);
            }
            catch (FormatException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid vehicle ID format"));
            }
            catch (Exception ex) when (!(ex is RpcException))
            {
                _logger.LogError(ex, "Error creating alert threshold");
                throw new RpcException(new Status(StatusCode.Internal, "Internal error occurred"));
            }
        }

        public override async Task<AlertThresholdResponse> UpdateAlertThreshold(
            UpdateAlertThresholdRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Updating alert threshold with ID: {Id}", request.Id);

            try
            {
                var alertThresholdDto = new AlertThresholdDto
                {
                    Id = Guid.Parse(request.Id),
                    VehicleId = Guid.Parse(request.VehicleId),
                    ParameterName = request.ParameterName,
                    MinValue = request.MinValue,
                    MaxValue = request.MaxValue,
                    IsEnabled = request.IsEnabled,
                    AlertMessage = request.AlertMessage,
                    Severity = (Domain.Enums.AlertSeverity)request.Severity
                };

                await _alertThresholdService.UpdateAlertThresholdAsync(alertThresholdDto);

                // Fetch the updated threshold to return
                var updatedThreshold = await _alertThresholdService.GetAlertThresholdByIdAsync(Guid.Parse(request.Id));

                if (updatedThreshold == null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, $"Alert threshold with ID {request.Id} not found after update"));
                }

                return MapToAlertThresholdResponse(updatedThreshold);
            }
            catch (FormatException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid ID format"));
            }
            catch (KeyNotFoundException)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Alert threshold with ID {request.Id} not found"));
            }
            catch (Exception ex) when (!(ex is RpcException))
            {
                _logger.LogError(ex, "Error updating alert threshold");
                throw new RpcException(new Status(StatusCode.Internal, "Internal error occurred"));
            }
        }

        public override async Task<DeleteAlertThresholdResponse> DeleteAlertThreshold(
            DeleteAlertThresholdRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Deleting alert threshold with ID: {Id}", request.Id);

            try
            {
                await _alertThresholdService.DeleteAlertThresholdAsync(Guid.Parse(request.Id));

                return new DeleteAlertThresholdResponse
                {
                    Success = true,
                    Message = $"Alert threshold with ID {request.Id} successfully deleted"
                };
            }
            catch (FormatException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid ID format"));
            }
            catch (KeyNotFoundException)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Alert threshold with ID {request.Id} not found"));
            }
            catch (Exception ex) when (!(ex is RpcException))
            {
                _logger.LogError(ex, "Error deleting alert threshold");
                throw new RpcException(new Status(StatusCode.Internal, "Internal error occurred"));
            }
        }

        // Helper methods
        private TelemetryDataResponse MapToTelemetryDataResponse(TelemetryDataDto telemetryData)
        {
            return new TelemetryDataResponse
            {
                Id = telemetryData.Id.ToString(),
                VehicleId = telemetryData.VehicleId.ToString(),
                Timestamp = Timestamp.FromDateTime(DateTime.SpecifyKind(telemetryData.Timestamp, DateTimeKind.Utc)),
                Latitude = telemetryData.Latitude,
                Longitude = telemetryData.Longitude,
                Speed = telemetryData.Speed,
                FuelLevel = telemetryData.FuelLevel,
                EngineTemperature = telemetryData.EngineTemperature,
                BatteryVoltage = telemetryData.BatteryVoltage,
                EngineRpm = telemetryData.EngineRpm,
                CheckEngineLightOn = telemetryData.CheckEngineLightOn,
                OdometerReading = telemetryData.OdometerReading,
                DiagnosticCode = telemetryData.DiagnosticCode
            };
        }

        private AlertThresholdResponse MapToAlertThresholdResponse(AlertThresholdDto threshold)
        {
            return new AlertThresholdResponse
            {
                Id = threshold.Id.ToString(),
                VehicleId = threshold.VehicleId.ToString(),
                ParameterName = threshold.ParameterName,
                MinValue = threshold.MinValue,
                MaxValue = threshold.MaxValue,
                IsEnabled = threshold.IsEnabled,
                AlertMessage = threshold.AlertMessage,
                Severity = (AlertSeverity)threshold.Severity
            };
        }

        private TelemetryDataDto SimulateNewTelemetryData(Guid vehicleId)
        {
            // This is a simple simulation function for streaming example
            // In a real application, you'd get actual data from sensors
            var random = new Random();

            return new TelemetryDataDto
            {
                Id = Guid.NewGuid(),
                VehicleId = vehicleId,
                Timestamp = DateTime.UtcNow,
                Latitude = 40.7128 + (random.NextDouble() - 0.5) * 0.01,
                Longitude = -74.0060 + (random.NextDouble() - 0.5) * 0.01,
                Speed = random.Next(0, 120),
                FuelLevel = random.Next(10, 100),
                EngineTemperature = 80 + random.Next(-10, 50),
                BatteryVoltage = 12 + (random.NextDouble() - 0.5) * 2,
                EngineRpm = random.Next(700, 5000),
                CheckEngineLightOn = random.Next(0, 20) == 0, // 5% chance
                OdometerReading = 50000 + random.Next(0, 100) / 10.0,
                DiagnosticCode = random.Next(0, 50) == 0 ? $"P{random.Next(1000, 9999)}" : null // Occasional diagnostic code
            };
        }

    }
}
