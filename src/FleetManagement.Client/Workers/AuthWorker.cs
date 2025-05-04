using Auth.Api.Protos;
using FleetManagement.Client.Services;
using NotificationService.API.Protos;

namespace FleetManagement.Client.Workers
{
    internal class AuthWorker : BackgroundService
    {
        private readonly ILogger<AuthWorker> _logger;
        private readonly AuthServiceClient _authServiceClient;
        private readonly RoleServiceClient _roleServiceClient;
        private readonly ClaimsServiceClient _claimsServiceClient;
        private readonly NotificationServiceClient _notificationServiceClient;

        public AuthWorker(AuthServiceClient authServiceClient, RoleServiceClient roleServiceClient, ClaimsServiceClient claimsServiceClient,
            NotificationServiceClient notificationServiceClient, ILogger<AuthWorker> logger)

        {
            _authServiceClient = authServiceClient;
            _roleServiceClient = roleServiceClient;
            _claimsServiceClient = claimsServiceClient;
            _notificationServiceClient = notificationServiceClient;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!stoppingToken.IsCancellationRequested)
            {
                var loginResponse = await Login();
                if (loginResponse?.IsAuthenticated != true || string.IsNullOrEmpty(loginResponse.Token))
                {
                    _logger.LogError("Failed to obtain authentication token. Aborting worker.");
                    return;
                }

                string token = loginResponse.Token;
                string userId = loginResponse.UserId;

                // Test AuthService methods
                //await Register();
                //await RefreshToken(loginResponse.Token, loginResponse.RefreshToken);
                await GetUser(userId, token);
                //await GetAllUsers(token);
                //await DeleteUser(userId, token);

                //// Test RoleService methods
                //await CreateRole(token);
                //await GetAllRoles(token);
                //await AddUserToRole(userId, token);
                //await GetUserRoles(userId, token);
                //await RemoveUserFromRole(userId, token);

                //// Test ClaimsService methods
                //await AddClaim(userId, token);
                //await GetUserClaims(userId, token);
                //await RemoveClaim(userId, token);
            }
        }

        private async Task<LoginResponse> Login()
        {
            var request = new LoginRequest
            {
                Email = "ramy@example.com",
                Password = "P@ssw0rd!"
            };

            try
            {
                _logger.LogInformation($"Logging in user: {request.Email}");
                var response = await _authServiceClient.LoginAsync(request);
                if (response.IsAuthenticated == true)
                {
                    Console.WriteLine($"Login successful: User={response.UserName}, Token={response.Token}");
                }
                else
                {
                    Console.WriteLine($"Login failed: {response.Message}");
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error logging in user: {request.Email}");
                Console.WriteLine($"Error logging in: {ex.Message}");
                return null;
            }
        }

        private async Task Register()
        {
            var request = new RegisterRequest
            {
                UserName = "Samy",
                Email = "Samy@example.com",
                Password = "P@ssw0rd!",
                PhoneNumber = "1234567890"
            };

            try
            {
                _logger.LogInformation($"Registering user: {request.Email}");
                var response = await _authServiceClient.RegisterAsync(request);
                if (response.IsAuthenticated == true)
                {
                    Console.WriteLine($"Registration successful: User={response.UserName}");
                    await SendNotification("user-123", "User Registered", $"User {response.UserName} registered successfully.");
                }
                else
                {
                    Console.WriteLine($"Registration failed: {response.Message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error registering user: {request.Email}");
                Console.WriteLine($"Error registering user: {ex.Message}");
            }
        }

        private async Task RefreshToken(string expiredToken, string refreshToken)
        {
            var request = new RefreshTokenRequest
            {
                ExpiredToken = expiredToken,
                RefreshToken = refreshToken
            };

            try
            {
                _logger.LogInformation("Refreshing token");
                var response = await _authServiceClient.RefreshTokenAsync(request);
                if (response.IsAuthenticated == true)
                {
                    Console.WriteLine($"Token refreshed: New Token={response.Token}");
                }
                else
                {
                    Console.WriteLine($"Token refresh failed: {response.Message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                Console.WriteLine($"Error refreshing token: {ex.Message}");
            }
        }

        private async Task GetUser(string userId, string token)
        {
            try
            {
                _logger.LogInformation($"Getting user: {userId}");
                var response = await _authServiceClient.GetUserAsync(userId, token);
                Console.WriteLine($"User retrieved: ID={response.Id}, UserName={response.UserName}, Email={response.Email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user: {userId}");
                Console.WriteLine($"Error getting user: {ex.Message}");
            }
        }

        private async Task GetAllUsers(string token)
        {
            try
            {
                _logger.LogInformation("Getting all users");
                var users = await _authServiceClient.GetAllUsersAsync(token);
                Console.WriteLine($"Retrieved {users.Users.Count()} users:");
                foreach (var user in users.Users)
                {
                    Console.WriteLine($"ID={user.Id}, UserName={user.UserName}, Email={user.Email}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                Console.WriteLine($"Error getting all users: {ex.Message}");
            }
        }

        private async Task DeleteUser(string userId, string token)
        {
            try
            {
                _logger.LogInformation($"Deleting user: {userId}");
                var response = await _authServiceClient.DeleteUserAsync(userId, token);
                if (response.Success)
                {
                    Console.WriteLine($"User deleted: ID={userId}");
                    await SendNotification(userId, "User Deleted", $"User {userId} was deleted successfully.");
                }
                else
                {
                    Console.WriteLine($"Failed to delete user: {userId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user: {userId}");
                Console.WriteLine($"Error deleting user: {ex.Message}");
            }
        }

        private async Task CreateRole(string token)
        {
            var request = new CreateRoleRequest
            {
                RoleName = "Driver"
            };

            try
            {
                _logger.LogInformation($"Creating role: {request.RoleName}");
                var response = await _roleServiceClient.CreateRoleAsync(request, token);
                if (response.Success)
                {
                    Console.WriteLine($"Role created: ID={response.Id}, Name={response.Name}");
                }
                else
                {
                    Console.WriteLine($"Failed to create role: {request.RoleName}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating role: {request.RoleName}");
                Console.WriteLine($"Error creating role: {ex.Message}");
            }
        }

        private async Task GetAllRoles(string token)
        {
            try
            {
                _logger.LogInformation("Getting all roles");
                var roles = await _roleServiceClient.GetAllRolesAsync(token);
                Console.WriteLine($"Retrieved {roles.Count} roles:");
                foreach (var role in roles)
                {
                    Console.WriteLine($"ID={role.Id}, Name={role.Name}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all roles");
                Console.WriteLine($"Error getting all roles: {ex.Message}");
            }
        }

        private async Task AddUserToRole(string userId, string token)
        {
            var request = new AddUserToRoleRequest
            {
                UserId = userId,
                RoleName = "Driver"
            };

            try
            {
                _logger.LogInformation($"Adding user {userId} to role {request.RoleName}");
                var response = await _roleServiceClient.AddUserToRoleAsync(request, token);
                if (response.Success)
                {
                    Console.WriteLine($"User {userId} added to role {request.RoleName}");
                    await SendNotification(userId, "Role Assigned", $"User {userId} was assigned the {request.RoleName} role.");
                }
                else
                {
                    Console.WriteLine($"Failed to add user {userId} to role {request.RoleName}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding user {userId} to role {request.RoleName}");
                Console.WriteLine($"Error adding user to role: {ex.Message}");
            }
        }

        private async Task RemoveUserFromRole(string userId, string token)
        {
            var request = new RemoveUserFromRoleRequest
            {
                UserId = userId,
                RoleName = "Driver"
            };

            try
            {
                _logger.LogInformation($"Removing user {userId} from role {request.RoleName}");
                var response = await _roleServiceClient.RemoveUserFromRoleAsync(request, token);
                if (response.Success)
                {
                    Console.WriteLine($"User {userId} removed from role {request.RoleName}");
                    await SendNotification(userId, "Role Removed", $"User {userId} was removed from the {request.RoleName} role.");
                }
                else
                {
                    Console.WriteLine($"Failed to remove user {userId} from role {request.RoleName}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing user {userId} from role {request.RoleName}");
                Console.WriteLine($"Error removing user from role: {ex.Message}");
            }
        }

        private async Task GetUserRoles(string userId, string token)
        {
            try
            {
                _logger.LogInformation($"Getting roles for user: {userId}");
                var response = await _roleServiceClient.GetUserRolesAsync(userId, token);
                Console.WriteLine($"Roles for user {userId}:");
                foreach (var role in response.Roles)
                {
                    Console.WriteLine($"ID={role.Id}, Name={role.Name}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting roles for user: {userId}");
                Console.WriteLine($"Error getting user roles: {ex.Message}");
            }
        }

        private async Task AddClaim(string userId, string token)
        {
            var request = new AddClaimRequest
            {
                UserId = userId,
                ClaimType = "Permission",
                ClaimValue = "ManageVehicles"
            };

            try
            {
                _logger.LogInformation($"Adding claim {request.ClaimType}:{request.ClaimValue} to user {userId}");
                var response = await _claimsServiceClient.AddClaimAsync(request, token);
                if (response.Success)
                {
                    Console.WriteLine($"Claim {request.ClaimType}:{request.ClaimValue} added to user {userId}");
                    await SendNotification(userId, "Claim Added", $"Claim {request.ClaimType}:{request.ClaimValue} was added to user {userId}.");
                }
                else
                {
                    Console.WriteLine($"Failed to add claim to user {userId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding claim to user {userId}");
                Console.WriteLine($"Error adding claim: {ex.Message}");
            }
        }

        private async Task RemoveClaim(string userId, string token)
        {
            var request = new RemoveClaimRequest
            {
                UserId = userId,
                ClaimType = "Permission",
                ClaimValue = "ManageVehicles"
            };

            try
            {
                _logger.LogInformation($"Removing claim {request.ClaimType}:{request.ClaimValue} from user {userId}");
                var response = await _claimsServiceClient.RemoveClaimAsync(request, token);
                if (response.Success)
                {
                    Console.WriteLine($"Claim {request.ClaimType}:{request.ClaimValue} removed from user {userId}");
                    await SendNotification(userId, "Claim Removed", $"Claim {request.ClaimType}:{request.ClaimValue} was removed from user {userId}.");
                }
                else
                {
                    Console.WriteLine($"Failed to remove claim from user {userId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing claim from user {userId}");
                Console.WriteLine($"Error removing claim: {ex.Message}");
            }
        }

        private async Task GetUserClaims(string userId, string token)
        {
            try
            {
                _logger.LogInformation($"Getting claims for user: {userId}");
                var response = await _claimsServiceClient.GetUserClaimsAsync(userId, token);
                Console.WriteLine($"Claims for user {userId}:");
                foreach (var claim in response.Claims)
                {
                    Console.WriteLine($"Type={claim.Type}, Value={claim.Value}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting claims for user: {userId}");
                Console.WriteLine($"Error getting user claims: {ex.Message}");
            }
        }

        private async Task SendNotification(string userId, string title, string body)
        {
            var notification = new SendNotificationRequest
            {
                RecipientIds = { userId },
                Type = NotificationType.SystemAlert,
                Priority = NotificationPriority.Normal,
                Title = title,
                Body = body,
                Channels = { NotificationChannel.Email }
            };

            try
            {
                var response = await _notificationServiceClient.SendNotificationAsync(notification);
                if (response.Success)
                {
                    _logger.LogInformation($"Notification sent: ID={response.NotificationId}");
                }
                else
                {
                    _logger.LogWarning($"Failed to send notification: {response.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending notification to user {userId}");
            }
        }
    }
}