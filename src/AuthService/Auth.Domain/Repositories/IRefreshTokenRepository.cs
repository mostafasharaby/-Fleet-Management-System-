using Auth.Domain.Models;

namespace Auth.Domain.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task StoreRefreshTokenAsync(string userId, string refreshToken, DateTime expiryTime);
        Task<RefreshToken> GetRefreshTokenAsync(string refreshToken, string userId);
        Task RevokeRefreshTokenAsync(string refreshToken);
        Task UpdateRefreshTokenAsync(RefreshToken token);

        //Task<RefreshToken> GetByTokenAsync(string token);
        //Task<IEnumerable<RefreshToken>> GetByUserIdAsync(Guid userId);
        //Task AddAsync(RefreshToken token);
        //Task DeleteAsync(Guid id);
    }
}
