using NotificationService.Domain.Enums;
using NotificationService.Domain.Models;

namespace NotificationService.Domain.Repositories
{
    public interface INotificationTemplateRepository
    {
        Task<NotificationTemplate> GetByIdAsync(string id);
        Task<IEnumerable<NotificationTemplate>> GetByTypeAsync(NotificationType type);
        Task<NotificationTemplate> AddAsync(NotificationTemplate template);
        Task UpdateAsync(NotificationTemplate template);
    }
}
