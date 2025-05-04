using Auth.Application.Interfaces;
using Auth.Application.Response;
using Auth.Domain.Models;
using Auth.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Auth.Application.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;
        //  private readonly AuthDbContext _context;                 // to follow DDD the Application layer should not depend on Infrastruction layer so i make IUserRepo , IRefreshTokenRepo in Domain => :)
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        public JwtService(IConfiguration configuration, UserManager<AppUser> userManager, IUserRepository userService, IRefreshTokenRepository refreshTokenRepository)
        {
            _configuration = configuration;
            _userManager = userManager;
            _userRepository = userService;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<AuthResponse> GenerateJwtToken(AppUser user)
        {
            ValidateUser(user);

            var claims = GetClaimsForUser(user);
            var signingCredentials = GetSigningCredentials();

            var accessTokenExpiry = DateTime.UtcNow.AddMinutes(20);
            var refreshTokenExpiry = DateTime.UtcNow.AddHours(12);
            var (accessToken, tokenExpirationTime) = CreateJwtToken(claims, signingCredentials, accessTokenExpiry);
            var refreshToken = GenerateRefreshToken();

            await UpdateUserTokens(user, accessToken, tokenExpirationTime, refreshToken, refreshTokenExpiry);
            await StoreRefreshToken(user, refreshToken, refreshTokenExpiry);
            return new AuthResponse
            {
                Token = accessToken,
                TokenExpiryTime = tokenExpirationTime,
                RefreshToken = refreshToken,
                RefreshTokenExpiryTime = refreshTokenExpiry,
                UserName = user.UserName,
                UserId = user.Id,
                IsAuthenticated = true,
                Message = "Authentication successful"
            };
        }

        private void ValidateUser(AppUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrEmpty(user.Id))
                throw new ArgumentNullException(nameof(user.Id), "User Id cannot be null or empty");
            if (string.IsNullOrEmpty(user.Email))
                throw new ArgumentNullException(nameof(user.Email), "User Email cannot be null or empty");
            if (string.IsNullOrEmpty(user.UserName))
                throw new ArgumentNullException(nameof(user.UserName), "User Name cannot be null or empty");
        }

        private List<Claim> GetClaimsForUser(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.UserName !),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var roles = _userManager.GetRolesAsync(user).Result;
            var userClaims = _userManager.GetClaimsAsync(user).Result;
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            claims.AddRange(userClaims);
            return claims;
        }


        private SigningCredentials GetSigningCredentials()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!));
            return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        }
        private (string accessToken, DateTime TokenExpires) CreateJwtToken(IEnumerable<Claim> claims, SigningCredentials credentials, DateTime expiryTime)
        {
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:validissuer"],
                audience: _configuration["Jwt:validaudience"],
                claims: claims,
                expires: expiryTime,
                signingCredentials: credentials);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            var tokenExpires = token.ValidTo;

            return (accessToken, tokenExpires);
        }


        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public async Task<AuthResponse> RefreshToken(string expiredToken, string refreshToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!);

            try
            {

                var principal = GetClaimsPrincipalFromExpiredToken(expiredToken, tokenHandler, key);

                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    throw new SecurityTokenException("Invalid token");


                var user = await _userManager.FindByIdAsync(userId);
                var storedToken = await ValidateRefreshToken(user!, refreshToken);


                storedToken.IsUsed = true;
                storedToken.ReplacedByToken = GenerateRefreshToken();
                await _refreshTokenRepository.UpdateRefreshTokenAsync(storedToken);

                var refreshTokenExpiry = DateTime.UtcNow.AddMinutes(2);
                var (newAccessToken, newAccessTokenExpiry) = CreateJwtToken(principal.Claims, GetSigningCredentials(), refreshTokenExpiry);
                var newRefreshToken = GenerateRefreshToken();


                await UpdateUserTokens(user!, newAccessToken, newAccessTokenExpiry, newRefreshToken, refreshTokenExpiry);
                await StoreRefreshToken(user!, newRefreshToken, DateTime.UtcNow.AddHours(12));
                await RevokeRefreshTokenAsync(refreshToken);

                return new AuthResponse
                {
                    Token = newAccessToken,
                    RefreshToken = newRefreshToken,
                    TokenExpiryTime = newAccessTokenExpiry,
                    RefreshTokenExpiryTime = refreshTokenExpiry,
                    UserName = user!.UserName,
                    IsAuthenticated = true,
                    Message = "Token refreshed successfully"
                };
            }
            catch (Exception ex)
            {
                throw new SecurityTokenException($"Invalid token: {ex.Message}");
            }
        }

        private async Task UpdateUserTokens(AppUser user, string accessToken, DateTime accessTokenExpiry, string refreshToken, DateTime refreshTokenExpiry)
        {
            user.Token = accessToken;
            user.TokenExpiryTime = accessTokenExpiry;
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = refreshTokenExpiry;
            await _userRepository.UpdateAsync(user);
        }
        private async Task StoreRefreshToken(AppUser user, string refreshToken, DateTime refreshTokenExpiry)
        {
            var refreshTokenEntity = new RefreshToken(refreshToken, refreshTokenExpiry, false, false, user.Id);

            //_context.RefreshTokens.Add(refreshTokenEntity);
            //await _context.SaveChangesAsync();
            await _refreshTokenRepository.StoreRefreshTokenAsync(user.Id, refreshToken, refreshTokenExpiry);
        }

        public async Task RevokeRefreshTokenAsync(string token)
        {
            //var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
            //if (refreshToken != null)
            //{
            //    refreshToken.IsRevoked = true;
            //    await _context.SaveChangesAsync();
            //}
            await _refreshTokenRepository.RevokeRefreshTokenAsync(token);
        }
        private async Task<RefreshToken> ValidateRefreshToken(AppUser user, string refreshToken)
        {
            //var storedToken = await _context.RefreshTokens
            //    .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.AppUserId == user.Id);

            var storedToken = await _refreshTokenRepository.GetRefreshTokenAsync(refreshToken, user.Id);
            if (storedToken == null || storedToken.IsUsed || storedToken.IsRevoked || storedToken.ExpiryTime <= DateTime.UtcNow)
                throw new SecurityTokenException("Invalid or expired refresh token.");

            return storedToken!;
        }


        private ClaimsPrincipal GetClaimsPrincipalFromExpiredToken(string token, JwtSecurityTokenHandler tokenHandler, byte[] key)
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false, // Allow expired tokens
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:validissuer"],
                ValidAudience = _configuration["Jwt:validaudience"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            }, out SecurityToken validatedToken);

            if (validatedToken is JwtSecurityToken jwtTokens && jwtTokens.ValidTo > DateTime.UtcNow)
                throw new SecurityTokenException("Access token is still valid and donot not need to be refreshed.");

            if (!(validatedToken is JwtSecurityToken jwtToken) || jwtToken.Header.Alg != SecurityAlgorithms.HmacSha256)
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
    }
}
