using Microsoft.Extensions.Logging;
using NotificationService.Domain.Enums;
using NotificationService.Domain.Models;
using NotificationService.Domain.Repositories;

namespace NotificationService.Infrastructure.Services
{
    public class SmsService : INotificationChannel
    {
        private readonly ILogger<SmsService> _logger;

        public SmsService(ILogger<SmsService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public NotificationChannel ChannelType => NotificationChannel.SMS;

        public async Task<bool> SendAsync(Notification notification)
        {
            try
            {
                // Get phone number from user service or metadata
                string phoneNumber = notification.Content.Metadata.ContainsKey("phone")
                    ? notification.Content.Metadata["phone"]
                    : await GetPhoneFromUserServiceAsync(notification.RecipientId);

                if (string.IsNullOrEmpty(phoneNumber))
                {
                    _logger.LogWarning($"Phone number not found for recipient {notification.RecipientId}");
                    return false;
                }

                // In a real implementation, this would call an SMS provider API
                // Mock implementation for demonstration purposes
                await Task.Delay(100); // Simulate network call

                _logger.LogInformation($"SMS sent to {phoneNumber} for notification {notification.Id}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send SMS for notification {notification.Id}");
                return false;
            }
        }

        private async Task<string> GetPhoneFromUserServiceAsync(string userId)
        {
            // In a real implementation, this would call the authentication service via gRPC
            // to get the user's phone number

            // Mock implementation for demonstration purposes
            await Task.Delay(10); // Simulate network call
            return $"+1555{new Random().Next(1000000, 9999999)}";
        }
    }

}
