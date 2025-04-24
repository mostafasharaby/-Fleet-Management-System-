using Grpc.Core;
using Grpc.Net.Client;
using VehicleService.API;
namespace FleetManagement.Client
{
    internal class VehicleClient : BackgroundService
    {
        private readonly GrpcChannel _channel;
        private readonly ILogger<VehicleClient> _logger;
        private VehicleService.API.VehicleService.VehicleServiceClient _client;

        public VehicleClient(ILogger<VehicleClient> logger)
        {
            _channel = GrpcChannel.ForAddress("https://localhost:7206");
            _client = new VehicleService.API.VehicleService.VehicleServiceClient(_channel);
            _logger = logger;
        }

        public async Task<VehicleResponse> GetVehicleAsync(string vehicleId)
        {
            try
            {
                var request = new GetVehicleRequest { VehicleId = vehicleId };
                var response = await _client.GetVehicleAsync(request);
                Console.WriteLine("response :", response);
                return response;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                _logger.LogWarning($"Vehicle with ID {vehicleId} not found");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching vehicle with ID {vehicleId}");
                throw;
            }
        }

        public async Task<ListVehiclesResponse> ListVehiclesAsync(
          int pageSize, int pageNumber, string filter = null, VehicleStatus? status = null)
        {
            try
            {
                var request = new ListVehiclesRequest
                {
                    PageSize = pageSize,
                    PageNumber = pageNumber,
                    Filter = filter ?? string.Empty,
                    Status = status.HasValue ? (VehicleService.API.VehicleStatus)status.Value : VehicleService.API.VehicleStatus.StatusUnknown
                };
                return await _client.ListVehiclesAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing vehicles");
                throw;
            }
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await GetVehicleAsync("8DB05DA5-9AE5-46D7-BC3C-10F260EAB20C");
            }
        }
    }

}
