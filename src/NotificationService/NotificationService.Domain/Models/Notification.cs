using NotificationService.Domain.Enums;
using NotificationService.Domain.ValueObjects;

namespace NotificationService.Domain.Models
{
    public class Notification
    {
        public Guid Id { get; private set; }
        public string RecipientId { get; private set; }
        public NotificationType Type { get; private set; }
        public NotificationPriority Priority { get; private set; }
        public NotificationContent Content { get; private set; }
        public NotificationStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? SentAt { get; private set; }
        public DateTime? ReadAt { get; private set; }
        public List<NotificationChannel> SentVia { get; private set; }
        public string TemplateId { get; private set; }

        // For EF Core
        private Notification() { }

        public Notification(
            string recipientId,
            NotificationType type,
            NotificationPriority priority,
            NotificationContent content,
            List<NotificationChannel> sentVia = null,
            string templateId = null)
        {
            Id = Guid.NewGuid();
            RecipientId = !string.IsNullOrEmpty(recipientId) ? recipientId : throw new ArgumentException("RecipientId cannot be null or empty");
            Type = type;
            Priority = priority;
            Content = content ?? throw new ArgumentNullException(nameof(content));
            Status = NotificationStatus.Pending;
            CreatedAt = DateTime.UtcNow;
            SentVia = sentVia ?? new List<NotificationChannel> { NotificationChannel.InApp };
            TemplateId = templateId;
        }

        public void MarkAsSent()
        {
            Status = NotificationStatus.Sent;
            SentAt = DateTime.UtcNow;
        }

        public void MarkAsRead()
        {
            Status = NotificationStatus.Read;
            ReadAt = DateTime.UtcNow;
        }

        public void MarkAsFailed()
        {
            Status = NotificationStatus.Failed;
        }
    }

}
