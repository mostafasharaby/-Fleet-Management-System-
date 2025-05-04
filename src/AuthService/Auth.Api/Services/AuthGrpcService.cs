using Auth.Api.Protos;
using Auth.Application.Interfaces;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace Auth.Api.Services
{
    public class AuthGrpcService : AuthService.AuthServiceBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthGrpcService> _logger;
        private readonly IMapper _mapper;

        public AuthGrpcService(IAuthService authService, ILogger<AuthGrpcService> logger, IMapper mapper)
        {
            _authService = authService;
            _logger = logger;
            _mapper = mapper;
        }

        public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
        {
            try
            {
                var appRequest = new Application.DTOs.LoginRequest
                {
                    Email = request.Email,
                    Password = request.Password
                };

                var result = await _authService.LoginAsync(appRequest);
                var response = new LoginResponse
                {
                    IsAuthenticated = result.IsAuthenticated ?? false,
                    Message = result.Message ?? "",
                    UserName = result.UserName ?? "",
                    UserId = result.UserId ?? "",
                };

                if (result.IsAuthenticated == true)
                {
                    response.Token = result.Token;
                    response.RefreshToken = result.RefreshToken;

                    if (result.TokenExpiryTime.HasValue)
                        response.TokenExpiryTime = Timestamp.FromDateTime(result.TokenExpiryTime.Value.ToUniversalTime());

                    if (result.RefreshTokenExpiryTime.HasValue)
                        response.RefreshTokenExpiryTime = Timestamp.FromDateTime(result.RefreshTokenExpiryTime.Value.ToUniversalTime());
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during gRPC login call");
                throw new RpcException(new Status(StatusCode.Internal, "An error occurred during login."));
            }
        }

        public override async Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
        {
            try
            {
                var appRequest = new Application.DTOs.RegisterRequest
                {
                    UserName = request.UserName,
                    Email = request.Email,
                    Password = request.Password,
                    PhoneNumber = request.PhoneNumber
                };
                //var appRequest = _mapper.Map<Application.DTOs.RegisterRequest>(request);

                var result = await _authService.RegisterAsync(appRequest);

                return new RegisterResponse
                {
                    IsAuthenticated = result.IsAuthenticated ?? false,
                    Message = result.Message ?? "",
                    UserName = result.UserName ?? ""
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during gRPC register call");
                throw new RpcException(new Status(StatusCode.Internal, "An error occurred during registration."));
            }
        }

        public override async Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest request, ServerCallContext context)
        {
            try
            {
                var appRequest = new Application.DTOs.RefreshTokenRequest
                {
                    ExpiredToken = request.ExpiredToken,
                    RefreshToken = request.RefreshToken
                };

                var result = await _authService.RefreshTokenAsync(appRequest);

                var response = new RefreshTokenResponse
                {
                    IsAuthenticated = result.IsAuthenticated ?? false,
                    Message = result.Message ?? "",
                    UserName = result.UserName ?? ""
                };

                if (result.IsAuthenticated == true)
                {
                    response.Token = result.Token;
                    response.RefreshToken = result.RefreshToken;

                    if (result.TokenExpiryTime.HasValue)
                        response.TokenExpiryTime = Timestamp.FromDateTime(result.TokenExpiryTime.Value.ToUniversalTime());

                    if (result.RefreshTokenExpiryTime.HasValue)
                        response.RefreshTokenExpiryTime = Timestamp.FromDateTime(result.RefreshTokenExpiryTime.Value.ToUniversalTime());
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during gRPC refresh token call");
                throw new RpcException(new Status(StatusCode.Internal, "An error occurred during token refresh."));
            }
        }

        [Authorize(Roles = "user")]
        public override async Task<UserResponse> GetUser(GetUserRequest request, ServerCallContext context)
        {
            try
            {
                var user = await _authService.GetUserByIdAsync(request.UserId);
                if (user == null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "User not found."));
                }

                return new UserResponse
                {
                    Id = user.Id ?? "",
                    UserName = user.UserName ?? "",
                    Email = user.Email ?? "",
                    PhoneNumber = user.PhoneNumber ?? ""
                };
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during gRPC get user call");
                throw new RpcException(new Status(StatusCode.Internal, "An error occurred while retrieving user data."));
            }
        }

        [Authorize(Roles = "admin")]
        public override async Task<UsersResponse> GetAllUsers(Empty request, ServerCallContext context)
        {
            try
            {
                var users = await _authService.GetAllUsersAsync();
                var response = new UsersResponse();

                foreach (var user in users)
                {
                    response.Users.Add(new UserResponse
                    {
                        Id = user.Id ?? "",
                        UserName = user.UserName ?? "",
                        Email = user.Email ?? "",
                        PhoneNumber = user.PhoneNumber ?? ""
                    });
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during gRPC get all users call");
                throw new RpcException(new Status(StatusCode.Internal, "An error occurred while retrieving users."));
            }
        }

        //[Authorize(Roles = "Admin")]
        //public override async Task<DeleteUserResponse> DeleteUser(DeleteUserRequest request, ServerCallContext context)
        //{
        //    try
        //    {
        //        var result = await _authService.DeleteUserAsync(request.UserId);
        //        return new DeleteUserResponse
        //        {
        //            Success = result
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error during gRPC delete user call");
        //        throw new RpcException(new Status(StatusCode.Internal, "An error occurred while deleting the user."));
        //    }
        //}
    }
}
