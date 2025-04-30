using Auth.Api.Protos;
using Auth.Application.Interfaces;
using Auth.Domain.Repositories;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace Auth.Api.Services
{
    public class AuthGrpcService : AuthService.AuthServiceBase
    {
        private readonly IAuthService _authService;
        private readonly IRoleRepository _roleRepository;
        private readonly IClaimsRepository _claimsRepository;
        private readonly ILogger<AuthGrpcService> _logger;

        public AuthGrpcService(
            IAuthService authService,
            IRoleRepository roleService,
            IClaimsRepository claimsService,
            ILogger<AuthGrpcService> logger)
        {
            _authService = authService;
            _roleRepository = roleService;
            _claimsRepository = claimsService;
            _logger = logger;
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
                _logger.LogError(ex, "Error during gRPC login call");
                throw new RpcException(new Status(StatusCode.Internal, "An error occurred during login."));
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

        [Authorize]
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

        //[Authorize(Roles = "Admin")]
        //public override async Task<UsersResponse> GetAllUsers(Empty request, ServerCallContext context)
        //{
        //    try
        //    {
        //        var users = await _authService.GetAllUsersAsync();
        //        var response = new UsersResponse();

        //        foreach (var user in users)
        //        {
        //            response.Users.Add(new UserResponse
        //            {
        //                Id = user.Id ?? "",
        //                UserName = user.UserName ?? "",
        //                Email = user.Email ?? "",
        //                PhoneNumber = user.PhoneNumber ?? ""
        //            });
        //        }

        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error during gRPC get all users call");
        //        throw new RpcException(new Status(StatusCode.Internal, "An error occurred while retrieving users."));
        //    }
        //}

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
