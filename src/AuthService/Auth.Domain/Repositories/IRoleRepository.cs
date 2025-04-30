using Auth.Domain.Models;

namespace Auth.Domain.Repositories
{
    public interface IRoleRepository
    {
        Task<List<RoleResponse>> GetAllRolesAsync();
        Task<UserRoles> GetUserRolesDetailsAsync(string userId);
        Task<RoleResponse> CreateRoleAsync(string roleName);
        Task<bool> AssignRoleAsync(string userId, string roleName);
        Task<bool> UpdateRoleAsync(string roleId, string newRoleName);
        Task<bool> RoleExistsAsync(string roleName);
        Task<bool> RemoveAllRolesAsync(string userId);
        Task<bool> RemoveUserFromRoleAsync(string userId, string roleName);

    }
}
