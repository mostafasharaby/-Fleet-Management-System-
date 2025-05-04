using FleetManagement.Client.Services;
using NotificationService.API.Protos;
using NotificationService.Domain.Models;

namespace FleetManagement.Client.Workers
{
    internal class NotificationWorker : BackgroundService
    {
        private readonly ILogger<NotificationWorker> _logger;
        private readonly NotificationServiceClient _notificationServiceClient;


        public NotificationWorker(NotificationServiceClient notificationServiceClient, ILogger<NotificationWorker> logger)
        {
            _notificationServiceClient = notificationServiceClient;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!stoppingToken.IsCancellationRequested)
            {
                //await SendNotification();
                //await CreateTemplate();
                //await GetNotificationHistory();
                //await MarkAsRead();
                //await SubscribeToNotifications(stoppingToken);
            }
        }

        private async Task SendNotification()
        {
            string userId = "user-123";
            var request = new SendNotificationRequest
            {
                RecipientIds = { userId },
                Type = NotificationType.VehicleAlert,
                Priority = NotificationPriority.Urgent,
                Title = "Vehicle Alert: High Engine Temperature",
                Body = "Vehicle ID 8DB05DA5-9AE5-46D7-BC3C-10F260EAB20C has engine temperature above 100°C.",
                Metadata = { { "vehicle_id", "8DB05DA5-9AE5-46D7-BC3C-10F260EAB20C" }, { "severity", "Critical" } },
                Channels = { NotificationChannel.Email, NotificationChannel.Push },
                TemplateId = "",
                TemplateData = { }
            };

            try
            {
                _logger.LogInformation($"Sending notification to user: {userId}");
                var response = await _notificationServiceClient.SendNotificationAsync(request);

                if (response.Success)
                {
                    Console.WriteLine($"Notification Sent: ID={response.NotificationId}");
                    Console.WriteLine($"Failed Recipients: {(response.FailedRecipients.Count > 0 ? string.Join(", ", response.FailedRecipients) : "None")}");
                }
                else
                {
                    Console.WriteLine($"Failed to send notification: {response.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending notification to user {userId}");
                Console.WriteLine($"Error sending notification: {ex.Message}");
            }
        }

        private async Task CreateTemplate()
        {
            var template = new NotificationTemplate
            {
                Name = "VehicleAlertTemplate",
                Description = "Template for vehicle alert notifications",
                Type = NotificationService.Domain.Enums.NotificationType.VehicleAlert,
                TitleTemplate = "Vehicle Alert: {Parameter}",
                BodyTemplate = "Vehicle {VehicleId} has {Parameter} out of range: {Value}.",
                DefaultMetadata = new Dictionary<string, string> { { "category", "VehicleMonitoring" } }
            };

            try
            {
                _logger.LogInformation($"Creating notification template: {template.Name}");
                var response = await _notificationServiceClient.CreateTemplateAsync(template);

                if (response.Success)
                {
                    Console.WriteLine($"Template Created: ID={response.TemplateId}");
                }
                else
                {
                    Console.WriteLine($"Failed to create template: {response.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating template {template.Name}");
                Console.WriteLine($"Error creating template: {ex.Message}");
            }
        }

        private async Task GetNotificationHistory()
        {
            string userId = "user-123";
            var request = new GetNotificationHistoryRequest
            {
                UserId = userId,
                Page = 1,
                PageSize = 5,
                IncludeRead = true,
                FilterType = NotificationType.VehicleAlert,
                //FromTimestamp =  new DateTimeOffset(DateTime.UtcNow.AddDays(-7)).ToUnixTimeSeconds(), -> has no value 
                //ToTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()
            };

            try
            {
                _logger.LogInformation($"Fetching notification history for user: {userId}");
                var response = await _notificationServiceClient.GetNotificationHistoryAsync(request);

                Console.WriteLine($"Notification History for User {userId}:");
                Console.WriteLine($"Total Count: {response.TotalCount}, Unread Count: {response.UnreadCount}");
                foreach (var notification in response.Notifications)
                {
                    Console.WriteLine($"Notification ID: {notification.NotificationId}");
                    Console.WriteLine($"Type: {notification.Type}");
                    Console.WriteLine($"Priority: {notification.Priority}");
                    Console.WriteLine($"Title: {notification.Title}");
                    Console.WriteLine($"Body: {notification.Body}");
                    Console.WriteLine($"Created: {notification.CreatedTimestamp}");
                    Console.WriteLine($"Read: {notification.IsRead}");
                    Console.WriteLine($"Channels: {string.Join(", ", notification.SentVia)}");
                    Console.WriteLine("---");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving notification history for user {userId}");
                Console.WriteLine($"Error retrieving notification history: {ex.Message}");
            }
        }

        private async Task MarkAsRead()
        {
            string userId = "user-123";
            var request = new MarkAsReadRequest
            {
                UserId = userId,
                NotificationIds = { "notification-1" },
                MarkAll = false
            };

            try
            {
                _logger.LogInformation($"Marking notifications as read for user: {userId}");
                var response = await _notificationServiceClient.MarkAsReadAsync(request);

                if (response.Success)
                {
                    Console.WriteLine($"Marked {response.UpdatedCount} notifications as read.");
                }
                else
                {
                    Console.WriteLine($"Failed to mark notifications as read: {response.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error marking notifications as read for user {userId}");
                Console.WriteLine($"Error marking notifications as read: {ex.Message}");
            }
        }

        private async Task SubscribeToNotifications(CancellationToken cancellationToken)
        {
            string userId = "user-123";
            var request = new SubscriptionRequest
            {
                UserId = userId,
                Types_ = { NotificationType.VehicleAlert, NotificationType.MaintenanceReminder },
                MinPriority = NotificationPriority.High
            };

            try
            {
                _logger.LogInformation($"Starting notification subscription for user: {userId}");
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(TimeSpan.FromSeconds(10));

                await _notificationServiceClient.SubscribeToNotificationsAsync(
                    request,
                    notification =>
                    {
                        Console.WriteLine($"Received Notification for User {notification.RecipientId}:");
                        Console.WriteLine($"ID: {notification.NotificationId}");
                        Console.WriteLine($"Type: {notification.Type}");
                        Console.WriteLine($"Priority: {notification.Priority}");
                        Console.WriteLine($"Title: {notification.Title}");
                        Console.WriteLine($"Body: {notification.Body}");
                        Console.WriteLine($"Created: {notification.CreatedTimestamp}");
                        Console.WriteLine("---");
                    },
                    cts.Token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error subscribing to notifications for user {userId}");
                Console.WriteLine($"Error subscribing to notifications: {ex.Message}");
            }
        }
    }
}