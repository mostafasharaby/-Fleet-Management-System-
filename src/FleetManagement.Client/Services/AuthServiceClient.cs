using Auth.Api.Protos;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;

namespace FleetManagement.Client.Services
{
    public class AuthServiceClient : IDisposable
    {
        private readonly GrpcChannel _channel;
        private readonly AuthService.AuthServiceClient _client;
        private readonly ILogger<AuthServiceClient> _logger;
        private readonly IMapper _mapper;

        public AuthServiceClient(string serviceUrl, ILogger<AuthServiceClient> logger, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _channel = GrpcChannel.ForAddress(serviceUrl);
            _client = new AuthService.AuthServiceClient(_channel);
            _logger.LogInformation($"AuthServiceClient initialized with endpoint: {serviceUrl}");
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            try
            {
                _logger.LogDebug($"Registering user: {request.Email}");
                var response = await _client.RegisterAsync(request);
                return _mapper.Map<RegisterResponse>(response);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogWarning($"Invalid register request: {ex.Status.Detail}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error registering user: {request.Email}");
                throw;
            }
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                _logger.LogDebug($"Logging in user: {request.Email}");
                var response = await _client.LoginAsync(request);
                return _mapper.Map<LoginResponse>(response);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogWarning($"Invalid login request: {ex.Status.Detail}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error logging in user: {request.Email}");
                throw;
            }
        }

        public async Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            try
            {
                _logger.LogDebug($"Refreshing token for token: {request.ExpiredToken}");
                //var protoRequest = _mapper.Map<Auth.Api.Protos.RefreshTokenRequest>(request);
                var response = await _client.RefreshTokenAsync(request);
                return _mapper.Map<RefreshTokenResponse>(response);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogWarning($"Invalid refresh token request: {ex.Status.Detail}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error refreshing token");
                throw;
            }
        }

        public async Task<UserResponse> GetUserAsync(string userId, string token)
        {
            try
            {
                _logger.LogDebug($"Getting user: {userId}");
                var metadata = new Metadata { { "Authorization", $"Bearer {token}" } };
                var response = await _client.GetUserAsync(new GetUserRequest { UserId = userId }, metadata);
                return _mapper.Map<UserResponse>(response);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.PermissionDenied)
            {
                _logger.LogWarning($"Permission Denied for that  User: {userId}");
                throw;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Unauthenticated)
            {
                _logger.LogWarning($"Unauthorized access for user: {userId}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user: {userId}");
                throw;
            }
        }

        public async Task<UsersResponse> GetAllUsersAsync(string token)
        {
            try
            {
                _logger.LogDebug("Getting all users");
                var metadata = new Metadata { { "Authorization", $"Bearer " + token } };
                // $"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjQyNjRjMjUyLWE3NjgtNDE5My04MTFkLWVjYjZhZDAyZDVlNyIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6ImFkbWluQGV4YW1wbGUuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZSI6ImFkbWluIiwianRpIjoiNGMzY2U2NWUtNzg1ZC00ZDkzLWJiNTYtNjVkYzEzNzgwYTI4IiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiYWRtaW4iLCJleHAiOjE3NDYzODUwNzcsImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjcwNTYiLCJhdWQiOiJodHRwOi8vbG9jYWxob3N0OjQyMDAifQ.mk766ZZEyJvBP7kp16ZNtc7yFvEyG-loGd5LjNo1tfY"
                var response = await _client.GetAllUsersAsync(new Empty(), metadata);
                return _mapper.Map<UsersResponse>(response);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Unauthenticated)
            {
                _logger.LogWarning("Unauthorized access for getting all users");
                throw;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.PermissionDenied)
            {
                _logger.LogWarning("PermissionDenied access for getting all users");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                throw;
            }
        }

        public async Task<DeleteUserResponse> DeleteUserAsync(string userId, string token)
        {
            try
            {
                _logger.LogDebug($"Deleting user: {userId}");
                var metadata = new Metadata { { "Authorization", $"Bearer {token}" } };
                var response = await _client.DeleteUserAsync(new DeleteUserRequest { UserId = userId }, metadata);
                return _mapper.Map<DeleteUserResponse>(response);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Unauthenticated)
            {
                _logger.LogWarning($"Unauthorized access for deleting user: {userId}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user: {userId}");
                throw;
            }
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
}