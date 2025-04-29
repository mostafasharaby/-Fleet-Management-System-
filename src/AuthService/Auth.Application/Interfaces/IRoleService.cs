namespace Auth.Application.Interfaces
{
    public interface IRoleService
    {
        Task<RoleResponse> CreateRoleAsync(string roleName);
        Task<List<RoleResponse>> GetAllRolesAsync();
        Task<bool> AddUserToRoleAsync(string userId, string roleName);
        Task<bool> RemoveUserFromRoleAsync(string userId, string roleName);
        Task<UserRoles> GetUserRolesAsync(string userId);
    }
}
