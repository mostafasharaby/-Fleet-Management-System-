using NotificationService.Domain.Enums;

namespace NotificationService.Application.DTOs
{
    public class NotificationDto
    {
        public string Id { get; set; }
        public string RecipientId { get; set; }
        public NotificationType Type { get; set; }
        public NotificationPriority Priority { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
        public NotificationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public List<NotificationChannel> SentVia { get; set; }
    }

}
