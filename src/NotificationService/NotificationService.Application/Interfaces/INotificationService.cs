using NotificationService.Application.DTOs;
using NotificationService.Domain.Enums;
using NotificationService.Domain.Models;

namespace NotificationService.Application.Interfaces
{
    public interface INotificationService
    {
        Task<NotificationDto> SendNotificationAsync(
            string recipientId,
            NotificationType type,
            NotificationPriority priority,
            string title,
            string body,
            Dictionary<string, string> metadata = null,
            List<NotificationChannel> channels = null);

        Task<IEnumerable<NotificationDto>> SendBulkNotificationsAsync(
            IEnumerable<string> recipientIds,
            NotificationType type,
            NotificationPriority priority,
            string title,
            string body,
            Dictionary<string, string> metadata = null,
            List<NotificationChannel> channels = null);

        Task<NotificationDto> SendTemplatedNotificationAsync(
            string recipientId,
            string templateId,
            Dictionary<string, string> templateData,
            NotificationPriority? priority = null,
            List<NotificationChannel> channels = null);

        Task<IEnumerable<NotificationDto>> SendBulkTemplatedNotificationsAsync(
            IEnumerable<string> recipientIds,
            string templateId,
            Dictionary<string, string> templateData,
            NotificationPriority? priority = null,
            List<NotificationChannel> channels = null);

        Task<NotificationDto> GetNotificationByIdAsync(string id);

        Task<NotificationCreationResponse> CreateNotificationTemplate(NotificationTemplate notificationTemplate);
        Task<(IEnumerable<NotificationDto> Notifications, int TotalCount, int UnreadCount)> GetNotificationHistoryAsync(
            string recipientId,
            int page,
            int pageSize,
            bool includeRead,
            NotificationType? filterType = null,
            DateTime? fromDate = null,
            DateTime? toDate = null);

        Task<int> MarkAsReadAsync(string recipientId, IEnumerable<string> notificationIds, bool markAll = false);
    }
}
