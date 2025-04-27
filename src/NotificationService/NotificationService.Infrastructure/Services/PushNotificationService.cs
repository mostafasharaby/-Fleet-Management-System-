using Microsoft.Extensions.Logging;
using NotificationService.Domain.Enums;
using NotificationService.Domain.Models;
using NotificationService.Domain.Repositories;

namespace NotificationService.Infrastructure.Services
{
    public class PushNotificationService : INotificationChannel
    {
        private readonly ILogger<PushNotificationService> _logger;

        public PushNotificationService(ILogger<PushNotificationService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public NotificationChannel ChannelType => NotificationChannel.Push;

        public async Task<bool> SendAsync(Notification notification)
        {
            try
            {
                // Get device tokens from user service or metadata
                var deviceTokens = notification.Content.Metadata.ContainsKey("deviceTokens")
                    ? notification.Content.Metadata["deviceTokens"].Split(',')
                    : await GetDeviceTokensFromUserServiceAsync(notification.RecipientId);

                if (deviceTokens == null || deviceTokens.Length == 0)
                {
                    _logger.LogWarning($"No device tokens found for recipient {notification.RecipientId}");
                    return false;
                }

                // In a real implementation, this would call a push notification service (Firebase, etc.)
                // Mock implementation for demonstration purposes
                await Task.Delay(100); // Simulate network call

                _logger.LogInformation($"Push notification sent to {deviceTokens.Length} devices for recipient {notification.RecipientId} and notification {notification.Id}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send push notification for notification {notification.Id}");
                return false;
            }
        }

        private async Task<string[]> GetDeviceTokensFromUserServiceAsync(string userId)
        {
            // In a real implementation, this would call the authentication service via gRPC
            // to get the user's device tokens

            // Mock implementation for demonstration purposes
            await Task.Delay(10); // Simulate network call
            return new[] { Guid.NewGuid().ToString("N") };
        }
    }
}
