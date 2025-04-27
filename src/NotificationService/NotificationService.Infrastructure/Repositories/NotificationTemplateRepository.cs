using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Enums;
using NotificationService.Domain.Models;
using NotificationService.Domain.Repositories;
using NotificationService.Infrastructure.Data;
namespace NotificationService.Infrastructure.Repositories
{
    public class NotificationTemplateRepository : INotificationTemplateRepository
    {
        private readonly NotificationDbContext _dbContext;

        public NotificationTemplateRepository(NotificationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<NotificationTemplate> GetByIdAsync(string id)
        {
            return await _dbContext.NotificationTemplates.FindAsync(id);
        }

        public async Task<IEnumerable<NotificationTemplate>> GetByTypeAsync(NotificationType type)
        {
            return await _dbContext.NotificationTemplates
                .Where(t => t.Type == type)
                .ToListAsync();
        }

        public async Task<NotificationTemplate> AddAsync(NotificationTemplate template)
        {
            await _dbContext.NotificationTemplates.AddAsync(template);
            await _dbContext.SaveChangesAsync();
            return template;
        }

        public async Task<NotificationTemplate> UpdateAsync(NotificationTemplate template)
        {
            _dbContext.NotificationTemplates.Update(template);
            await _dbContext.SaveChangesAsync();
            return template;
        }

        Task INotificationTemplateRepository.UpdateAsync(NotificationTemplate template)
        {
            return UpdateAsync(template);
        }
    }
}
