using NotificationService.Domain.Enums;
using NotificationService.Domain.Models;

namespace NotificationService.Domain.Repositories
{
    public interface INotificationRepository
    {
        Task<Notification> GetByIdAsync(Guid id);
        Task<IEnumerable<Notification>> GetByRecipientIdAsync(string recipientId, int page, int pageSize, bool includeRead);
        Task<int> GetUnreadCountByRecipientIdAsync(string recipientId);
        Task<int> GetTotalCountByRecipientIdAsync(string recipientId, bool includeRead);
        Task<Notification> AddAsync(Notification notification);
        Task UpdateAsync(Notification notification);
        Task<IEnumerable<Notification>> GetByFilterAsync(string recipientId, NotificationType? type, DateTime? fromDate, DateTime? toDate, int page, int pageSize);
        Task<int> MarkAsReadAsync(string recipientId, IEnumerable<Guid> notificationIds, bool markAll);
    }
}
