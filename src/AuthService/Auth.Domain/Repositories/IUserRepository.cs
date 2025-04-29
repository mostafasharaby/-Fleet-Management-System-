using Auth.Domain.Models;

namespace Auth.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<AppUser> GetByIdAsync(Guid id);
        Task<AppUser> GetByUsernameAsync(string username);
        Task<AppUser> GetByEmailAsync(string email);
        Task AddAsync(AppUser user);
        Task UpdateAsync(AppUser user);
        Task DeleteAsync(Guid id);
    }

}
