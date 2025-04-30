using Auth.Domain.Models;
using Auth.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Auth.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<RoleRepository> _logger;

        public RoleRepository(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, ILogger<RoleRepository> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<RoleResponse> CreateRoleAsync(string roleName)
        {
            try
            {
                var roleExists = await _roleManager.RoleExistsAsync(roleName);
                if (roleExists)
                {
                    return new RoleResponse
                    {
                        Id = null,
                        Name = null
                    };
                }

                var role = new IdentityRole(roleName);
                var result = await _roleManager.CreateAsync(role);
                if (!result.Succeeded)
                {
                    return new RoleResponse
                    {
                        Id = null,
                        Name = null
                    };
                }

                return new RoleResponse
                {
                    Id = role.Id,
                    Name = role.Name
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role");
                return new RoleResponse
                {
                    Id = null,
                    Name = null
                };
            }
        }

        public async Task<bool> AddUserToRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                return false;
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);
            return result.Succeeded;
        }

        public async Task<bool> RemoveUserFromRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            return result.Succeeded;
        }

        public async Task<UserRoles> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);
            var roleDetails = new List<RolesDetails>();

            foreach (var roleName in roles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                roleDetails.Add(new RolesDetails
                {
                    Id = role.Id,
                    Name = role.Name
                });
            }

            return new UserRoles
            {
                UserId = userId,
                RolesDetails = roleDetails
            };
        }

        public async Task<List<RoleResponse>> GetAllRolesAsync()
        {
            var roles = await _roleManager.Roles
                .Select(r => new RoleResponse { Id = r.Id, Name = r.Name })
                .ToListAsync();

            return roles;
        }

        public async Task<UserRoles> GetUserRolesDetailsAsync(string userId)
        {
            var roles = await _userManager.FindByIdAsync(userId);
            var roleDetails = _roleManager.Roles
                .Where(r => r.Name.Contains(r.Name))
                .ToList();

            var UserRoles = new UserRoles
            {
                UserId = userId,
                RolesDetails = roleDetails.Select(r => new RolesDetails
                {
                    Id = r.Id,
                    Name = r.Name
                }).ToList()
            };
            return UserRoles;
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            return await _roleManager.RoleExistsAsync(roleName);
        }

        public async Task<bool> RemoveAllRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var currentRoles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            return result.Succeeded;
        }

        public async Task<bool> AssignRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var result = await _userManager.AddToRoleAsync(user!, roleName);
            return result.Succeeded;
        }

        public async Task<bool> UpdateRoleAsync(string roleId, string newRoleName)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return false;

            role.Name = newRoleName;
            await _roleManager.UpdateAsync(role);
            return true;
        }
    }
}
