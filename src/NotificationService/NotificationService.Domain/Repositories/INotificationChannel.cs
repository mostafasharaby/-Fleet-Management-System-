using NotificationService.Domain.Enums;
using NotificationService.Domain.Models;

namespace NotificationService.Domain.Repositories
{
    public interface INotificationChannel
    {
        Task<bool> SendAsync(Notification notification);
        NotificationChannel ChannelType { get; }
    }
}
