using Auth.Domain.Models;

namespace Auth.Domain.Repositories
{
    public interface IClaimsRepository
    {
        Task<bool> AddClaimToUserAsync(string userId, string claimType, string claimValue);
        Task<bool> RemoveClaimFromUserAsync(string userId, string claimType, string claimValue);
        Task<UserClaims> GetUserClaimsAsync(string userId);
    }
}
