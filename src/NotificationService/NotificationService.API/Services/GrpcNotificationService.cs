using Grpc.Core;
using NotificationService.API.Protos;
using NotificationService.Application.Interfaces;
using System.Collections.Concurrent;

namespace NotificationService.API.Services
{
    public class NotificationGrpcService : Protos.NotificationService.NotificationServiceBase
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationGrpcService> _logger;
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<IServerStreamWriter<NotificationMessage>, NotificationSubscription>> _subscriptions =
            new ConcurrentDictionary<string, ConcurrentDictionary<IServerStreamWriter<NotificationMessage>, NotificationSubscription>>();

        public NotificationGrpcService(INotificationService notificationService, ILogger<NotificationGrpcService> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public override async Task<SendNotificationResponse> SendNotification(SendNotificationRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation($"Received request to send notification to {request.RecipientIds.Count} recipients");
                var failedRecipients = new List<string>();
                var channels = request.Channels.Select(c => (Domain.Enums.NotificationChannel)c).ToList();

                if (!channels.Any())
                {
                    channels.Add(Domain.Enums.NotificationChannel.InApp);
                }

                // Handle templated notifications
                if (!string.IsNullOrEmpty(request.TemplateId))
                {
                    var templateData = request.TemplateData.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    var priority = (Domain.Enums.NotificationPriority)request.Priority;

                    try
                    {
                        var result = await _notificationService.SendBulkTemplatedNotificationsAsync(
                            request.RecipientIds,
                            request.TemplateId,
                            templateData,
                            priority,
                            channels);

                        // Notify subscribers for each recipient
                        foreach (var notification in result)
                        {
                            await NotifySubscribersAsync(notification);
                        }

                        return new SendNotificationResponse
                        {
                            Success = true,
                            NotificationId = result.FirstOrDefault()?.Id
                        };
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to send templated notification: {ex.Message}");
                        return new SendNotificationResponse
                        {
                            Success = false,
                            ErrorMessage = ex.Message
                        };
                    }
                }
                else
                {
                    // Handle direct notifications
                    var metadata = request.Metadata.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    var notificationType = (Domain.Enums.NotificationType)request.Type;
                    var priority = (Domain.Enums.NotificationPriority)request.Priority;

                    try
                    {
                        var result = await _notificationService.SendBulkNotificationsAsync(
                            request.RecipientIds,
                            notificationType,
                            priority,
                            request.Title,
                            request.Body,
                            metadata,
                            channels);

                        // Notify subscribers for each recipient
                        foreach (var notification in result)
                        {
                            await NotifySubscribersAsync(notification);
                        }

                        return new SendNotificationResponse
                        {
                            Success = true,
                            NotificationId = result.FirstOrDefault()?.Id
                        };
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to send notification: {ex.Message}");
                        return new SendNotificationResponse
                        {
                            Success = false,
                            ErrorMessage = ex.Message
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in SendNotification: {ex.Message}");
                return new SendNotificationResponse
                {
                    Success = false,
                    ErrorMessage = $"Internal error: {ex.Message}"
                };
            }
        }

        public override async Task<CreateTemplateResponse> CreateTemplate(CreateTemplateRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation($"Received request to create notification template: {request.Name}");

                var notificationType = (Domain.Enums.NotificationType)request.Type;
                var defaultMetadata = request.DefaultMetadata.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                var template = new Domain.Models.NotificationTemplate(
                    request.Name,
                    request.Description,
                    notificationType,
                    request.TitleTemplate,
                    request.BodyTemplate,
                    defaultMetadata);

                var creationResponse = await _notificationService.CreateNotificationTemplate(template);
                return new CreateTemplateResponse
                {
                    Success = creationResponse.Success,
                    TemplateId = creationResponse.TempleteId
                };


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in CreateTemplate: {ex.Message}");
                return new CreateTemplateResponse
                {
                    Success = false,
                    ErrorMessage = $"Internal error: {ex.Message}"
                };
            }
        }

        public override async Task<GetNotificationHistoryResponse> GetNotificationHistory(GetNotificationHistoryRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation($"Received request to get notification history for user {request.UserId}");

                var fromDate = request.FromTimestamp > 0
                    ? DateTimeOffset.FromUnixTimeMilliseconds(request.FromTimestamp).UtcDateTime
                    : (DateTime?)null;

                var toDate = request.ToTimestamp > 0
                    ? DateTimeOffset.FromUnixTimeMilliseconds(request.ToTimestamp).UtcDateTime
                    : (DateTime?)null;

                var filterType = request.FilterType != 0
                    ? (Domain.Enums.NotificationType?)request.FilterType
                    : null;

                var (notifications, totalCount, unreadCount) = await _notificationService.GetNotificationHistoryAsync(
                    request.UserId,
                    request.Page,
                    request.PageSize,
                    request.IncludeRead,
                    filterType,
                    fromDate,
                    toDate);

                var response = new GetNotificationHistoryResponse
                {
                    TotalCount = totalCount,
                    UnreadCount = unreadCount
                };

                foreach (var notification in notifications)
                {
                    response.Notifications.Add(MapToNotificationMessage(notification));
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetNotificationHistory: {ex.Message}");
                return new GetNotificationHistoryResponse
                {
                    TotalCount = 0,
                    UnreadCount = 0
                };
            }
        }

        public override async Task<MarkAsReadResponse> MarkAsRead(MarkAsReadRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation($"Received request to mark notifications as read for user {request.UserId}");

                var updatedCount = await _notificationService.MarkAsReadAsync(
                    request.UserId,
                    request.NotificationIds,
                    request.MarkAll);

                return new MarkAsReadResponse
                {
                    Success = true,
                    UpdatedCount = updatedCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in MarkAsRead: {ex.Message}");
                return new MarkAsReadResponse
                {
                    Success = false,
                    ErrorMessage = $"Internal error: {ex.Message}"
                };
            }
        }

        public override async Task SubscribeToNotifications(SubscriptionRequest request, IServerStreamWriter<NotificationMessage> responseStream, ServerCallContext context)
        {

            var userId = request.UserId;
            _logger.LogInformation($"User {userId} subscribing to notifications");

            var subscription = new NotificationSubscription
            {
                UserId = userId,
                Types = request.Types_.Select(t => (Domain.Enums.NotificationType)t).ToList(),
                MinPriority = (Domain.Enums.NotificationPriority)request.MinPriority
            };

            // Add subscription to the dictionary
            var userSubscriptions = _subscriptions.GetOrAdd(userId,
                _ => new ConcurrentDictionary<IServerStreamWriter<NotificationMessage>, NotificationSubscription>());

            userSubscriptions[responseStream] = subscription;

            try
            {
                // Keep connection open until client disconnects
                await Task.Delay(Timeout.InfiniteTimeSpan, context.CancellationToken);
            }
            catch (TaskCanceledException)
            {
                // Client disconnected
                _logger.LogInformation($"User {userId} unsubscribed from notifications");
            }
            finally
            {
                // Remove subscription when connection is closed
                if (_subscriptions.TryGetValue(userId, out var subscriptions))
                {
                    subscriptions.TryRemove(responseStream, out _);

                    // Clean up if no more subscriptions for this user
                    if (subscriptions.IsEmpty)
                    {
                        _subscriptions.TryRemove(userId, out _);
                    }
                }
            }
        }

        // Helper method to map notification DTO to gRPC message
        private NotificationMessage MapToNotificationMessage(Application.DTOs.NotificationDto notification)
        {
            var message = new NotificationMessage
            {
                NotificationId = notification.Id,
                RecipientId = notification.RecipientId,
                Type = (Protos.NotificationType)notification.Type,
                Priority = (Protos.NotificationPriority)notification.Priority,
                Title = notification.Title,
                Body = notification.Body,
                CreatedTimestamp = new DateTimeOffset(notification.CreatedAt).ToUnixTimeMilliseconds(),
                IsRead = notification.Status == Domain.Enums.NotificationStatus.Read
            };

            // Add metadata
            foreach (var item in notification.Metadata)
            {
                message.Metadata.Add(item.Key, item.Value);
            }

            // Add channels
            foreach (var channel in notification.SentVia)
            {
                message.SentVia.Add((Protos.NotificationChannel)channel);
            }

            return message;
        }

        // Helper method to notify subscribers of new notifications
        private async Task NotifySubscribersAsync(Application.DTOs.NotificationDto notification)
        {
            if (!_subscriptions.TryGetValue(notification.RecipientId, out var userSubscriptions))
            {
                return; // No subscribers for this user
            }

            var notificationMessage = MapToNotificationMessage(notification);

            foreach (var subscription in userSubscriptions)
            {
                try
                {
                    var responseStream = subscription.Key;
                    var subscriptionDetails = subscription.Value;

                    // Check if the notification matches the subscription criteria
                    if (ShouldSendNotification(notification, subscriptionDetails))
                    {
                        await responseStream.WriteAsync(notificationMessage);
                        _logger.LogInformation($"Sent notification {notification.Id} to subscriber {notification.RecipientId}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to send notification to subscriber: {ex.Message}");
                }
            }
        }

        // Helper method to determine if a notification should be sent to a subscriber
        private bool ShouldSendNotification(Application.DTOs.NotificationDto notification, NotificationSubscription subscription)
        {
            // Check if the notification type is in the subscription's type list
            bool typeMatches = subscription.Types.Count == 0 || subscription.Types.Contains(notification.Type);

            // Check if the notification priority is at least the subscription's minimum priority
            bool priorityMatches = (int)notification.Priority >= (int)subscription.MinPriority;

            return typeMatches && priorityMatches;
        }
    }

    // Helper class to track subscription details
    internal class NotificationSubscription
    {
        public string UserId { get; set; }
        public List<Domain.Enums.NotificationType> Types { get; set; } = new List<Domain.Enums.NotificationType>();
        public Domain.Enums.NotificationPriority MinPriority { get; set; } = Domain.Enums.NotificationPriority.Normal;
    }
}