using Auth.Application.DTOs;
using Auth.Application.Response;

namespace Auth.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);
        //Task RevokeTokenAsync(string token, string userId);
        //Task<bool> ValidateTokenAsync(string token);
        Task<AppUserDto> GetUserByIdAsync(string userId);
        Task<List<AppUserDto>> GetAllUsersAsync();
    }
}
