using Auth.Application.Response;
using Microsoft.AspNetCore.Authentication;

namespace Auth.Application.Interfaces
{
    public interface IGoogleService
    {
        AuthenticationProperties GetGoogleLoginProperties(string redirectUri);
        Task<AuthResponse> GoogleLoginCallbackAsync();
    }
}
