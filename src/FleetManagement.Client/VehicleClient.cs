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

        //private VehicleService.API.VehicleService.VehicleServiceClient Client
        //{
        //    get
        //    {
        //        if (_client == null)
        //        {
        //            var channel = GrpcChannel.ForAddress("https://localhost:7206");
        //            _client = new VehicleService.API.VehicleService.VehicleServiceClient(channel);
        //        }
        //        return _client;
        //    }
        //}

        public async Task<VehicleResponse> GetVehicleAsync(Guid vehicleId)
        {
            try
            {
                var request = new GetVehicleRequest { VehicleId = "8DB05DA5-9AE5-46D7-BC3C-10F260EAB20C" };
                return await _client.GetVehicleAsync(request);
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
                await GetVehicleAsync(await Task.FromResult(Guid.NewGuid()));
            }
        }
    }

}
