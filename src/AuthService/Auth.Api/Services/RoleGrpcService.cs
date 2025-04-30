using Auth.Api.Protos;
using Auth.Domain.Repositories;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace Auth.Api.Services
{
    public class RoleGrpcService : RoleService.RoleServiceBase
    {
        private readonly IRoleRepository _roleRepository;
        private readonly ILogger<RoleGrpcService> _logger;

        public RoleGrpcService(IRoleRepository roleService, ILogger<RoleGrpcService> logger)
        {
            _roleRepository = roleService;
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        public override async Task<CreateRoleResponse> CreateRole(CreateRoleRequest request, ServerCallContext context)
        {
            try
            {
                var result = await _roleRepository.CreateRoleAsync(request.RoleName);
                return new CreateRoleResponse
                {
                    Id = result.Id ?? "",
                    Name = result.Name ?? "",
                    Success = !string.IsNullOrEmpty(result.Id)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during gRPC create role call");
                throw new RpcException(new Status(StatusCode.Internal, "An error occurred while creating the role."));
            }
        }

        [Authorize]
        public override async Task<RolesResponse> GetAllRoles(Empty request, ServerCallContext context)
        {
            try
            {
                var roles = await _roleRepository.GetAllRolesAsync();
                var response = new RolesResponse();

                foreach (var role in roles)
                {
                    response.Roles.Add(new RoleInfo
                    {
                        Id = role.Id ?? "",
                        Name = role.Name ?? ""
                    });
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during gRPC get all roles call");
                throw new RpcException(new Status(StatusCode.Internal, "An error occurred while retrieving roles."));
            }
        }

        [Authorize(Roles = "Admin")]
        public override async Task<AddUserToRoleResponse> AddUserToRole(AddUserToRoleRequest request, ServerCallContext context)
        {
            try
            {
                var result = await _roleRepository.AssignRoleAsync(request.UserId, request.RoleName);
                return new AddUserToRoleResponse
                {
                    Success = result
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during gRPC add user to role call");
                throw new RpcException(new Status(StatusCode.Internal, "An error occurred while adding the user to the role."));
            }
        }

        [Authorize(Roles = "Admin")]
        public override async Task<RemoveUserFromRoleResponse> RemoveUserFromRole(RemoveUserFromRoleRequest request, ServerCallContext context)
        {
            try
            {
                var result = await _roleRepository.RemoveUserFromRoleAsync(request.UserId, request.RoleName);
                return new RemoveUserFromRoleResponse
                {
                    Success = result
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during gRPC remove user from role call");
                throw new RpcException(new Status(StatusCode.Internal, "An error occurred while removing the user from the role."));
            }
        }

        [Authorize]
        public override async Task<UserRolesResponse> GetUserRoles(GetUserRolesRequest request, ServerCallContext context)
        {
            try
            {
                var userRoles = await _roleRepository.GetUserRolesDetailsAsync(request.UserId);
                if (userRoles == null)
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "User not found."));
                }

                var response = new UserRolesResponse
                {
                    UserId = userRoles.UserId ?? ""
                };

                if (userRoles.RolesDetails != null)
                {
                    foreach (var role in userRoles.RolesDetails)
                    {
                        response.Roles.Add(new RoleInfo
                        {
                            Id = role.Id ?? "",
                            Name = role.Name ?? ""
                        });
                    }
                }

                return response;
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during gRPC get user roles call");
                throw new RpcException(new Status(StatusCode.Internal, "An error occurred while retrieving user roles."));
            }
        }
    }
}
