using Auth.Application.DTOs;
using Auth.Application.Interfaces;
using Auth.Application.Response;
using Auth.Domain.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Auth.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthService> _logger;
        private readonly IMapper _mapper;

        public AuthService(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IJwtService jwtService,
            IEmailService emailService,
            ILogger<AuthService> logger,
             IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _emailService = emailService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var userExists = await _userManager.FindByNameAsync(request.UserName);
                if (userExists != null)
                {
                    return new AuthResponse
                    {
                        Message = "User already exists!",
                        IsAuthenticated = false
                    };
                }

                var user = new AppUser
                {
                    UserName = request.UserName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    return new AuthResponse
                    {
                        Message = $"User creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}",
                        IsAuthenticated = false
                    };
                }

                await _userManager.AddToRoleAsync(user, "User");

                var message = new Message(new[] { user.Email! }, "Welcome to Fleet Management System", "Your account has been created successfully!");
                _emailService.SendEmail(message);

                return new AuthResponse
                {
                    Message = "User created successfully!",
                    IsAuthenticated = true,
                    UserName = user.UserName
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration");
                return new AuthResponse
                {
                    Message = "Registration failed due to an internal error",
                    IsAuthenticated = false
                };
            }
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    return new AuthResponse
                    {
                        Message = "User not found!",
                        IsAuthenticated = false
                    };
                }

                var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, false);
                if (!result.Succeeded)
                {
                    return new AuthResponse
                    {
                        Message = "Invalid credentials!",
                        IsAuthenticated = false
                    };
                }

                var tokenResponse = await _jwtService.GenerateJwtToken(user);

                user.Token = tokenResponse.Token;
                user.TokenExpiryTime = tokenResponse.TokenExpiryTime;
                user.RefreshToken = tokenResponse.RefreshToken;
                user.RefreshTokenExpiryTime = tokenResponse.RefreshTokenExpiryTime;
                await _userManager.UpdateAsync(user);

                return tokenResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user login");
                return new AuthResponse
                {
                    Message = "Login failed due to an internal error",
                    IsAuthenticated = false
                };
            }
        }

        public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            return await _jwtService.RefreshToken(request.ExpiredToken, request.RefreshToken);
        }

        public async Task<AppUserDto> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            return new AppUserDto(user.Id, user.UserName, user.Email, user.PhoneNumber);
        }

        public async Task<List<AppUserDto>> GetAllUsersAsync()
        {
            var users = _userManager.Users.ToList();
            return users.Select(u => new AppUserDto(u.Id, u.UserName, u.Email, u.PhoneNumber)).ToList();
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        //public async Task RevokeTokenAsync(string token, string userId)
        //{
        //    var refreshToken = await _refreshTokenRepository.GetByTokenAsync(token);
        //    if (refreshToken != null && refreshToken.UserId == userId)
        //    {
        //        refreshToken.Revoke();
        //        await _refreshTokenRepository.UpdateAsync(refreshToken);
        //    }
        //}

        //public async Task<bool> ValidateTokenAsync(string token)
        //{
        //    return _jwtService.ValidateToken(token);
        //}


    }
}
