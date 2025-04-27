using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Enums;
using NotificationService.Domain.Models;
using NotificationService.Domain.Repositories;
using NotificationService.Infrastructure.Data;

namespace NotificationService.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly NotificationDbContext _dbContext;

        public NotificationRepository(NotificationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<Notification> GetByIdAsync(Guid id)
        {
            return await _dbContext.Notifications.FindAsync(id);
        }

        public async Task<IEnumerable<Notification>> GetByRecipientIdAsync(string recipientId, int page, int pageSize, bool includeRead)
        {
            var query = _dbContext.Notifications
                .Where(n => n.RecipientId == recipientId);

            if (!includeRead)
            {
                query = query.Where(n => n.Status != NotificationStatus.Read);
            }

            return await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountByRecipientIdAsync(string recipientId)
        {
            return await _dbContext.Notifications
                .CountAsync(n => n.RecipientId == recipientId && n.Status != NotificationStatus.Read);
        }

        public async Task<int> GetTotalCountByRecipientIdAsync(string recipientId, bool includeRead)
        {
            var query = _dbContext.Notifications
                .Where(n => n.RecipientId == recipientId);

            if (!includeRead)
            {
                query = query.Where(n => n.Status != NotificationStatus.Read);
            }

            return await query.CountAsync();
        }

        public async Task<Notification> AddAsync(Notification notification)
        {
            await _dbContext.Notifications.AddAsync(notification);
            await _dbContext.SaveChangesAsync();
            return notification;
        }

        public async Task UpdateAsync(Notification notification)
        {
            _dbContext.Notifications.Update(notification);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Notification>> GetByFilterAsync(
            string recipientId,
            NotificationType? type,
            DateTime? fromDate,
            DateTime? toDate,
            int page,
            int pageSize)
        {
            var query = _dbContext.Notifications
                .Where(n => n.RecipientId == recipientId);

            if (type.HasValue)
            {
                query = query.Where(n => n.Type == type.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(n => n.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(n => n.CreatedAt <= toDate.Value);
            }

            return await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> MarkAsReadAsync(string recipientId, IEnumerable<Guid> notificationIds, bool markAll)
        {
            var query = _dbContext.Notifications
                .Where(n => n.RecipientId == recipientId && n.Status != NotificationStatus.Read);

            if (!markAll && notificationIds != null && notificationIds.Any())
            {
                query = query.Where(n => notificationIds.Contains(n.Id));
            }

            var notifications = await query.ToListAsync();
            foreach (var notification in notifications)
            {
                notification.MarkAsRead();
            }

            await _dbContext.SaveChangesAsync();
            return notifications.Count;
        }
    }
}
