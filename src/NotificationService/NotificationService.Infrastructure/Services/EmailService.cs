using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotificationService.Domain.Enums;
using NotificationService.Domain.Models;
using NotificationService.Domain.Repositories;
using System.Net.Mail;

namespace NotificationService.Infrastructure.Services
{
    public class EmailService : INotificationChannel
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
        {
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public NotificationChannel ChannelType => NotificationChannel.Email;

        public async Task<bool> SendAsync(Notification notification)
        {
            try
            {
                // Get email address from user service or metadata
                string emailAddress = notification.Content.Metadata.ContainsKey("email")
                    ? notification.Content.Metadata["email"]
                    : await GetEmailFromUserServiceAsync(notification.RecipientId);

                if (string.IsNullOrEmpty(emailAddress))
                {
                    _logger.LogWarning($"Email address not found for recipient {notification.RecipientId}");
                    return false;
                }

                using var client = new SmtpClient(_settings.SmtpServer, _settings.SmtpPort)
                {
                    Credentials = new System.Net.NetworkCredential(_settings.SmtpUsername, _settings.SmtpPassword),
                    EnableSsl = true
                };

                using var message = new MailMessage
                {
                    From = new MailAddress(_settings.FromAddress, _settings.FromName),
                    Subject = notification.Content.Title,
                    Body = notification.Content.Body,
                    IsBodyHtml = true
                };

                message.To.Add(new MailAddress(emailAddress));

                await client.SendMailAsync(message);
                _logger.LogInformation($"Email sent to {emailAddress} for notification {notification.Id}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email for notification {notification.Id}");
                return false;
            }
        }

        private async Task<string> GetEmailFromUserServiceAsync(string userId)
        {
            // In a real implementation, this would call the authentication service via gRPC
            // to get the user's email address

            // Mock implementation for demonstration purposes
            await Task.Delay(10); // Simulate network call
            return $"{userId}@example.com";
        }
    }

}
