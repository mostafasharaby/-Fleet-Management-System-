using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Enums;
using NotificationService.Domain.Models;
using NotificationService.Domain.Repositories;
using NotificationService.Domain.ValueObjects;

namespace NotificationService.Application.Services
{
    public class NotificationAppService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly INotificationTemplateRepository _templateRepository;
        private readonly ITemplateProcessor _templateProcessor;
        private readonly NotificationChannelFactory _channelFactory;

        public NotificationAppService(
            INotificationRepository notificationRepository,
            INotificationTemplateRepository templateRepository,
            ITemplateProcessor templateProcessor,
            NotificationChannelFactory channelFactory)
        {
            _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
            _templateRepository = templateRepository ?? throw new ArgumentNullException(nameof(templateRepository));
            _templateProcessor = templateProcessor ?? throw new ArgumentNullException(nameof(templateProcessor));
            _channelFactory = channelFactory ?? throw new ArgumentNullException(nameof(channelFactory));
        }

        public async Task<NotificationDto> SendNotificationAsync(
            string recipientId,
            NotificationType type,
            NotificationPriority priority,
            string title,
            string body,
            Dictionary<string, string> metadata = null,
            List<NotificationChannel> channels = null)
        {
            var content = new NotificationContent(title, body, metadata);
            var notification = new Notification(recipientId, type, priority, content, channels);

            var result = await _notificationRepository.AddAsync(notification);

            var channelInstances = _channelFactory.GetChannels(channels ?? new List<NotificationChannel> { NotificationChannel.InApp });
            foreach (var channel in channelInstances)
            {
                try
                {
                    await channel.SendAsync(notification);
                }
                catch (Exception)
                {
                    // Log exception but continue with other channels
                }
            }

            notification.MarkAsSent();
            await _notificationRepository.UpdateAsync(notification);

            return MapToDto(notification);
        }

        public async Task<IEnumerable<NotificationDto>> SendBulkNotificationsAsync(
            IEnumerable<string> recipientIds,
            NotificationType type,
            NotificationPriority priority,
            string title,
            string body,
            Dictionary<string, string> metadata = null,
            List<NotificationChannel> channels = null)
        {
            var notifications = new List<NotificationDto>();

            foreach (var recipientId in recipientIds)
            {
                var notification = await SendNotificationAsync(recipientId, type, priority, title, body, metadata, channels);
                notifications.Add(notification);
            }

            return notifications;
        }

        public async Task<NotificationDto> SendTemplatedNotificationAsync(
            string recipientId,
            string templateId,
            Dictionary<string, string> templateData,
            NotificationPriority? priority = null,
            List<NotificationChannel> channels = null)
        {
            var template = await _templateRepository.GetByIdAsync(templateId);
            if (template == null)
            {
                throw new ArgumentException($"Template with ID {templateId} not found");
            }

            var (title, body) = await _templateProcessor.ProcessTemplateAsync(templateId, templateData);

            // Merge metadata
            var metadata = new Dictionary<string, string>(template.DefaultMetadata);
            if (templateData != null)
            {
                foreach (var kvp in templateData)
                {
                    if (!metadata.ContainsKey(kvp.Key))
                    {
                        metadata[kvp.Key] = kvp.Value;
                    }
                }
            }

            return await SendNotificationAsync(
                recipientId,
                template.Type,
                priority ?? NotificationPriority.Normal,
                title,
                body,
                metadata,
                channels);
        }

        public async Task<IEnumerable<NotificationDto>> SendBulkTemplatedNotificationsAsync(
            IEnumerable<string> recipientIds,
            string templateId,
            Dictionary<string, string> templateData,
            NotificationPriority? priority = null,
            List<NotificationChannel> channels = null)
        {
            var notifications = new List<NotificationDto>();

            foreach (var recipientId in recipientIds)
            {
                var notification = await SendTemplatedNotificationAsync(recipientId, templateId, templateData, priority, channels);
                notifications.Add(notification);
            }

            return notifications;
        }

        public async Task<NotificationDto> GetNotificationByIdAsync(string id)
        {
            var notification = await _notificationRepository.GetByIdAsync(Guid.Parse(id));
            return notification != null ? MapToDto(notification) : null;
        }

        public async Task<(IEnumerable<NotificationDto> Notifications, int TotalCount, int UnreadCount)> GetNotificationHistoryAsync(
            string recipientId,
            int page,
            int pageSize,
            bool includeRead,
            NotificationType? filterType = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var notifications = await _notificationRepository.GetByFilterAsync(
                recipientId,
                filterType,
                fromDate,
                toDate,
                page,
                pageSize);

            var totalCount = await _notificationRepository.GetTotalCountByRecipientIdAsync(recipientId, includeRead);
            var unreadCount = await _notificationRepository.GetUnreadCountByRecipientIdAsync(recipientId);

            return (notifications.Select(MapToDto), totalCount, unreadCount);
        }

        public async Task<int> MarkAsReadAsync(string recipientId, IEnumerable<string> notificationIds, bool markAll = false)
        {
            var notificationGuids = notificationIds?.Select(id => Guid.Parse(id)) ?? new List<Guid>();
            return await _notificationRepository.MarkAsReadAsync(recipientId, notificationGuids, markAll);
        }

        private NotificationDto MapToDto(Notification notification)
        {
            return new NotificationDto
            {
                Id = notification.Id.ToString(),
                RecipientId = notification.RecipientId,
                Type = notification.Type,
                Priority = notification.Priority,
                Title = notification.Content.Title,
                Body = notification.Content.Body,
                Metadata = notification.Content.Metadata,
                Status = notification.Status,
                CreatedAt = notification.CreatedAt,
                SentAt = notification.SentAt,
                ReadAt = notification.ReadAt,
                SentVia = notification.SentVia
            };
        }
    }
}
