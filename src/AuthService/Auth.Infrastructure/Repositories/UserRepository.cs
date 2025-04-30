using Auth.Domain.Models;
using Auth.Domain.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Auth.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<AppUser> _userManager;

        public UserRepository(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<AppUser> GetByIdAsync(Guid id)
        {
            return await _userManager.FindByIdAsync(id.ToString())
                ?? throw new KeyNotFoundException($"User with ID {id} not found.");
        }

        public async Task<AppUser> GetByUsernameAsync(string username)
        {
            return await _userManager.FindByNameAsync(username)
                ?? throw new KeyNotFoundException($"User with username {username} not found.");
        }

        public async Task<AppUser> GetByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email)
                ?? throw new KeyNotFoundException($"User with email {email} not found.");
        }

        public async Task AddAsync(AppUser user)
        {
            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
                throw new InvalidOperationException("Failed to add user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task UpdateAsync(AppUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new InvalidOperationException("Failed to update user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await GetByIdAsync(id);
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                throw new InvalidOperationException("Failed to delete user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
