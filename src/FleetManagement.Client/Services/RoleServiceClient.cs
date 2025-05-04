using Auth.Api.Protos;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;

namespace FleetManagement.Client.Services
{
    public class RoleServiceClient : IDisposable
    {
        private readonly GrpcChannel _channel;
        private readonly RoleService.RoleServiceClient _client;
        private readonly ILogger<RoleServiceClient> _logger;
        private readonly IMapper _mapper;

        public RoleServiceClient(string serviceUrl, ILogger<RoleServiceClient> logger, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _channel = GrpcChannel.ForAddress(serviceUrl);
            _client = new RoleService.RoleServiceClient(_channel);
            _logger.LogInformation($"RoleServiceClient initialized with endpoint: {serviceUrl}");
        }

        public async Task<CreateRoleResponse> CreateRoleAsync(CreateRoleRequest request, string token)
        {
            try
            {
                _logger.LogDebug($"Creating role: {request.RoleName}");
                var metadata = new Metadata { { "Authorization", $"Bearer {token}" } };
                var protoRequest = _mapper.Map<Auth.Api.Protos.CreateRoleRequest>(request);
                var response = await _client.CreateRoleAsync(protoRequest, metadata);
                return _mapper.Map<CreateRoleResponse>(response);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogWarning($"Invalid create role request: {ex.Status.Detail}");
                throw;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Unauthenticated)
            {
                _logger.LogWarning($"Unauthorized access for creating role: {request.RoleName}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating role: {request.RoleName}");
                throw;
            }
        }

        public async Task<List<RoleInfo>> GetAllRolesAsync(string token)
        {
            try
            {
                _logger.LogDebug("Getting all roles");
                var metadata = new Metadata { { "Authorization", $"Bearer {token}" } };
                var response = await _client.GetAllRolesAsync(new Empty(), metadata);
                return _mapper.Map<List<RoleInfo>>(response);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Unauthenticated)
            {
                _logger.LogWarning("Unauthorized access for getting all roles");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all roles");
                throw;
            }
        }

        public async Task<AddUserToRoleResponse> AddUserToRoleAsync(AddUserToRoleRequest request, string token)
        {
            try
            {
                _logger.LogDebug($"Adding user {request.UserId} to role {request.RoleName}");
                var metadata = new Metadata { { "Authorization", $"Bearer {token}" } };
                var protoRequest = _mapper.Map<Auth.Api.Protos.AddUserToRoleRequest>(request);
                var response = await _client.AddUserToRoleAsync(protoRequest, metadata);
                return _mapper.Map<AddUserToRoleResponse>(response);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogWarning($"Invalid add user to role request: {ex.Status.Detail}");
                throw;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Unauthenticated)
            {
                _logger.LogWarning($"Unauthorized access for adding user {request.UserId} to role");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding user {request.UserId} to role {request.RoleName}");
                throw;
            }
        }

        public async Task<RemoveUserFromRoleResponse> RemoveUserFromRoleAsync(RemoveUserFromRoleRequest request, string token)
        {
            try
            {
                _logger.LogDebug($"Removing user {request.UserId} from role {request.RoleName}");
                var metadata = new Metadata { { "Authorization", $"Bearer {token}" } };
                var protoRequest = _mapper.Map<Auth.Api.Protos.RemoveUserFromRoleRequest>(request);
                var response = await _client.RemoveUserFromRoleAsync(protoRequest, metadata);
                return _mapper.Map<RemoveUserFromRoleResponse>(response);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogWarning($"Invalid remove user from role request: {ex.Status.Detail}");
                throw;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Unauthenticated)
            {
                _logger.LogWarning($"Unauthorized access for removing user {request.UserId} from role");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing user {request.UserId} from role {request.RoleName}");
                throw;
            }
        }

        public async Task<UserRolesResponse> GetUserRolesAsync(string userId, string token)
        {
            try
            {
                _logger.LogDebug($"Getting roles for user: {userId}");
                var metadata = new Metadata { { "Authorization", $"Bearer {token}" } };
                var response = await _client.GetUserRolesAsync(new GetUserRolesRequest { UserId = userId }, metadata);
                return _mapper.Map<UserRolesResponse>(response);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                _logger.LogWarning($"User not found: {userId}");
                throw;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Unauthenticated)
            {
                _logger.LogWarning($"Unauthorized access for getting roles for user: {userId}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting roles for user: {userId}");
                throw;
            }
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
}