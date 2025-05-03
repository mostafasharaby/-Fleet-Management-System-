using Grpc.Core;
using Grpc.Net.Client;
using TelemetryService.API.Protos;

namespace FleetManagement.Client.Services
{
    public class TelemetryServiceClient : IDisposable
    {
        private readonly GrpcChannel _channel;
        private readonly TelemetryService.API.Protos.TelemetryService.TelemetryServiceClient _client;
        private readonly ILogger<TelemetryServiceClient> _logger;

        public TelemetryServiceClient(string serverUrl, ILogger<TelemetryServiceClient> logger)
        {
            _logger = logger;
            _channel = GrpcChannel.ForAddress(serverUrl);
            _client = new TelemetryService.API.Protos.TelemetryService.TelemetryServiceClient(_channel);

            _logger.LogInformation("TelemetryServiceClient initialized with server URL: {ServerUrl}", serverUrl);
        }

        public async Task<TelemetryDataResponse> GetTelemetryDataByIdAsync(string id)
        {
            _logger.LogInformation("Getting telemetry data with ID: {Id}", id);

            try
            {
                var request = new GetTelemetryDataByIdRequest { Id = id };
                return await _client.GetTelemetryDataByIdAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting telemetry data by ID {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<TelemetryDataResponse>> GetTelemetryDataByVehicleIdAsync(string vehicleId, int limit = 100)
        {
            _logger.LogInformation("Getting telemetry data for vehicle ID: {VehicleId}, limit: {Limit}", vehicleId, limit);

            try
            {
                var request = new GetTelemetryDataByVehicleIdRequest
                {
                    VehicleId = vehicleId,
                    Limit = limit
                };

                var response = await _client.GetTelemetryDataByVehicleIdAsync(request);
                return response.TelemetryData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting telemetry data for vehicle ID {VehicleId}", vehicleId);
                throw;
            }
        }

        public async Task<IEnumerable<TelemetryDataResponse>> GetTelemetryDataByTimeRangeAsync(
            string vehicleId, DateTime startTime, DateTime endTime)
        {
            _logger.LogInformation("Getting telemetry data for vehicle ID: {VehicleId} from {StartTime} to {EndTime}",
                vehicleId, startTime, endTime);

            try
            {
                var request = new GetTelemetryDataByTimeRangeRequest
                {
                    VehicleId = vehicleId,
                    StartTime = new DateTimeOffset(startTime).ToUnixTimeSeconds(),
                    EndTime = new DateTimeOffset(endTime).ToUnixTimeSeconds()
                };

                var response = await _client.GetTelemetryDataByTimeRangeAsync(request);
                return response.TelemetryData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting telemetry data for vehicle ID {VehicleId} in time range", vehicleId);
                throw;
            }
        }

        public async Task<TelemetryDataResponse> SendTelemetryDataAsync(TelemetryDataRequest telemetryData)
        {
            _logger.LogInformation("Sending telemetry data for vehicle ID: {VehicleId}", telemetryData.VehicleId);

            try
            {
                return await _client.SendTelemetryDataAsync(telemetryData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending telemetry data for vehicle ID {VehicleId}", telemetryData.VehicleId);
                throw;
            }
        }

        public async Task<BatchTelemetryDataResponse> SendBatchTelemetryDataAsync(List<TelemetryDataRequest> telemetryDataBatch)
        {
            _logger.LogInformation("Sending batch telemetry data with {Count} entries", telemetryDataBatch);

            try
            {
                var request = new BatchTelemetryDataRequest();
                request.TelemetryData.AddRange(telemetryDataBatch);

                return await _client.SendBatchTelemetryDataAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending batch telemetry data");
                throw;
            }
        }

        public async Task<IEnumerable<TelemetryDataResponse>> GetLatestTelemetryForAllVehiclesAsync()
        {
            _logger.LogInformation("Getting latest telemetry data for all vehicles");

            try
            {
                var request = new GetLatestTelemetryRequest();
                var response = await _client.GetLatestTelemetryForAllVehiclesAsync(request);

                return response.TelemetryData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting latest telemetry data for all vehicles");
                throw;
            }
        }

        public async Task StreamVehicleTelemetryAsync(string vehicleId, Func<TelemetryDataResponse, Task> onDataReceived, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting telemetry stream for vehicle ID: {VehicleId}", vehicleId);

            try
            {
                var request = new StreamVehicleTelemetryRequest { VehicleId = vehicleId };

                using var call = _client.StreamVehicleTelemetry(request, cancellationToken: cancellationToken);

                await foreach (var data in call.ResponseStream.ReadAllAsync(cancellationToken))
                {
                    await onDataReceived(data);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Telemetry stream for vehicle {VehicleId} was cancelled", vehicleId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error streaming telemetry data for vehicle ID {VehicleId}", vehicleId);
                throw;
            }
        }

        public async Task<IEnumerable<AlertThresholdResponse>> GetAlertThresholdsByVehicleIdAsync(string vehicleId)
        {
            _logger.LogInformation("Getting alert thresholds for vehicle ID: {VehicleId}", vehicleId);

            try
            {
                var request = new GetAlertThresholdsByVehicleIdRequest { VehicleId = vehicleId };
                var response = await _client.GetAlertThresholdsByVehicleIdAsync(request);

                return response.Thresholds;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting alert thresholds for vehicle ID {VehicleId}", vehicleId);
                throw;
            }
        }

        public async Task<AlertThresholdResponse> GetAlertThresholdByIdAsync(string id)
        {
            _logger.LogInformation("Getting alert threshold with ID: {Id}", id);

            try
            {
                var request = new GetAlertThresholdByIdRequest { Id = id };
                return await _client.GetAlertThresholdByIdAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting alert threshold by ID {Id}", id);
                throw;
            }
        }

        public async Task<AlertThresholdResponse> CreateAlertThresholdAsync(CreateAlertThresholdRequest threshold)
        {
            _logger.LogInformation("Creating alert threshold for vehicle ID: {VehicleId}, parameter: {ParameterName}",
                threshold.VehicleId, threshold.ParameterName);

            try
            {
                return await _client.CreateAlertThresholdAsync(threshold);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating alert threshold");
                throw;
            }
        }

        public async Task<AlertThresholdResponse> UpdateAlertThresholdAsync(UpdateAlertThresholdRequest threshold)
        {
            _logger.LogInformation("Updating alert threshold with ID: {Id}", threshold.Id);

            try
            {
                return await _client.UpdateAlertThresholdAsync(threshold);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating alert threshold with ID {Id}", threshold.Id);
                throw;
            }
        }

        public async Task<DeleteAlertThresholdResponse> DeleteAlertThresholdAsync(string id)
        {
            _logger.LogInformation("Deleting alert threshold with ID: {Id}", id);

            try
            {
                var request = new DeleteAlertThresholdRequest { Id = id };
                return await _client.DeleteAlertThresholdAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting alert threshold with ID {Id}", id);
                throw;
            }
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
}
