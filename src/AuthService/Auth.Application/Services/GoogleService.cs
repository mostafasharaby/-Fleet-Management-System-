using Auth.Application.Interfaces;
using Auth.Application.Response;
using Auth.Domain.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Auth.Application.Services
{
    public class GoogleService : IGoogleService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJwtService _jwtService;

        public GoogleService(UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor, IJwtService jwtService)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _jwtService = jwtService;
        }

        public AuthenticationProperties GetGoogleLoginProperties(string redirectUri)
        {
            return new AuthenticationProperties { RedirectUri = redirectUri };
        }

        public async Task<AuthResponse> GoogleLoginCallbackAsync()
        {
            var authenticateResult = await _httpContextAccessor.HttpContext!.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded)
            {
                throw new UnauthorizedAccessException("External login failed.");
            }

            var externalUser = authenticateResult.Principal;
            var email = externalUser.FindFirstValue(ClaimTypes.Email);
            var userName = $"{externalUser.FindFirstValue(ClaimTypes.Name)?.Replace(" ", "_")}_{Guid.NewGuid().ToString().Substring(0, 4)}";
            var user = await _userManager.FindByEmailAsync(email!);

            if (user == null)
            {
                user = new AppUser { UserName = userName, Email = email };
                var createUserResult = await _userManager.CreateAsync(user);
                Console.WriteLine(createUserResult);
                if (!createUserResult.Succeeded)
                {
                    throw new Exception("Error creating user.");
                }

                await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", externalUser.FindFirstValue(ClaimTypes.NameIdentifier)!, "Google"));
            }
            var authResponse = await _jwtService.GenerateJwtToken(user);
            return authResponse;
        }
    }
}
