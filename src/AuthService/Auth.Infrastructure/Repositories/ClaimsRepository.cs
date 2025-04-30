using Auth.Domain.Models;
using Auth.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Auth.Infrastructure.Repositories
{
    public class ClaimsRepository : IClaimsRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<IClaimsRepository> _logger;

        public ClaimsRepository(
            UserManager<AppUser> userManager,
            ILogger<IClaimsRepository> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<bool> AddClaimToUserAsync(string userId, string claimType, string claimValue)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return false;
                }

                var claim = new Claim(claimType, claimValue);
                var result = await _userManager.AddClaimAsync(user, claim);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding claim to user");
                return false;
            }
        }

        public async Task<bool> RemoveClaimFromUserAsync(string userId, string claimType, string claimValue)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return false;
                }

                var claim = new Claim(claimType, claimValue);
                var result = await _userManager.RemoveClaimAsync(user, claim);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing claim from user");
                return false;
            }
        }

        public async Task<UserClaims> GetUserClaimsAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            var claims = await _userManager.GetClaimsAsync(user);
            var claimDetails = claims.Select(c => new ClaimDetails
            {
                Type = c.Type,
                Value = c.Value
            }).ToList();

            return new UserClaims
            {
                UserId = userId,
                ClaimsDetails = claimDetails
            };
        }
    }
}
