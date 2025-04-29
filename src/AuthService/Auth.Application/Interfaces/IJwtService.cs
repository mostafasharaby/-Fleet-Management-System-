using Auth.Application.Response;
using Auth.Domain.Models;

namespace Auth.Application.Interfaces
{
    public interface IJwtService
    {
        Task<AuthResponse> GenerateJwtToken(AppUser user);
        Task<AuthResponse> RefreshToken(string expiredToken, string refreshToken);
    }
}
