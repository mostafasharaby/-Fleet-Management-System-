using Auth.Api.Protos;
using AutoMapper;
using Grpc.Core;
using Grpc.Net.Client;

namespace FleetManagement.Client.Services
{
    public class ClaimsServiceClient : IDisposable
    {
        private readonly GrpcChannel _channel;
        private readonly ClaimsService.ClaimsServiceClient _client;
        private readonly ILogger<ClaimsServiceClient> _logger;
        private readonly IMapper _mapper;

        public ClaimsServiceClient(string serviceUrl, ILogger<ClaimsServiceClient> logger, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _channel = GrpcChannel.ForAddress(serviceUrl);
            _client = new ClaimsService.ClaimsServiceClient(_channel);
            _logger.LogInformation($"ClaimsServiceClient initialized with endpoint: {serviceUrl}");
        }

        public async Task<AddClaimResponse> AddClaimAsync(AddClaimRequest request, string token)
        {
            try
            {
                _logger.LogDebug($"Adding claim {request.ClaimType}:{request.ClaimValue} to user {request.UserId}");
                var metadata = new Metadata { { "Authorization", $"Bearer {token}" } };
                var protoRequest = _mapper.Map<Auth.Api.Protos.AddClaimRequest>(request);
                var response = await _client.AddClaimAsync(protoRequest, metadata);
                return _mapper.Map<AddClaimResponse>(response);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogWarning($"Invalid add claim request: {ex.Status.Detail}");
                throw;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Unauthenticated)
            {
                _logger.LogWarning($"Unauthorized access for adding claim to user {request.UserId}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding claim {request.ClaimType}:{request.ClaimValue} to user {request.UserId}");
                throw;
            }
        }

        public async Task<RemoveClaimResponse> RemoveClaimAsync(RemoveClaimRequest request, string token)
        {
            try
            {
                _logger.LogDebug($"Removing claim {request.ClaimType}:{request.ClaimValue} from user {request.UserId}");
                var metadata = new Metadata { { "Authorization", $"Bearer {token}" } };
                var protoRequest = _mapper.Map<Auth.Api.Protos.RemoveClaimRequest>(request);
                var response = await _client.RemoveClaimAsync(protoRequest, metadata);
                return _mapper.Map<RemoveClaimResponse>(response);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogWarning($"Invalid remove claim request: {ex.Status.Detail}");
                throw;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Unauthenticated)
            {
                _logger.LogWarning($"Unauthorized access for removing claim from user {request.UserId}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing claim {request.ClaimType}:{request.ClaimValue} from user {request.UserId}");
                throw;
            }
        }

        public async Task<UserClaimsResponse> GetUserClaimsAsync(string userId, string token)
        {
            try
            {
                _logger.LogDebug($"Getting claims for user: {userId}");
                var metadata = new Metadata { { "Authorization", $"Bearer {token}" } };
                var response = await _client.GetUserClaimsAsync(new GetUserClaimsRequest { UserId = userId }, metadata);
                return _mapper.Map<UserClaimsResponse>(response);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                _logger.LogWarning($"User not found: {userId}");
                throw;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Unauthenticated)
            {
                _logger.LogWarning($"Unauthorized access for getting claims for user: {userId}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting claims for user: {userId}");
                throw;
            }
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
}