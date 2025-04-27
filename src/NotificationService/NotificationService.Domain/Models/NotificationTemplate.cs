using NotificationService.Domain.Enums;

namespace NotificationService.Domain.Models
{
    public class NotificationTemplate
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public NotificationType Type { get; private set; }
        public string TitleTemplate { get; private set; }
        public string BodyTemplate { get; private set; }
        public Dictionary<string, string> DefaultMetadata { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        // For EF Core
        private NotificationTemplate() { }

        public NotificationTemplate(
            string name,
            string description,
            NotificationType type,
            string titleTemplate,
            string bodyTemplate,
            Dictionary<string, string> defaultMetadata = null)
        {
            Id = Guid.NewGuid().ToString();
            Name = !string.IsNullOrEmpty(name) ? name : throw new ArgumentException("Name cannot be null or empty");
            Description = description;
            Type = type;
            TitleTemplate = !string.IsNullOrEmpty(titleTemplate) ? titleTemplate : throw new ArgumentException("TitleTemplate cannot be null or empty");
            BodyTemplate = !string.IsNullOrEmpty(bodyTemplate) ? bodyTemplate : throw new ArgumentException("BodyTemplate cannot be null or empty");
            DefaultMetadata = defaultMetadata ?? new Dictionary<string, string>();
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(
            string name,
            string description,
            string titleTemplate,
            string bodyTemplate,
            Dictionary<string, string> defaultMetadata = null)
        {
            Name = !string.IsNullOrEmpty(name) ? name : throw new ArgumentException("Name cannot be null or empty");
            Description = description;
            TitleTemplate = !string.IsNullOrEmpty(titleTemplate) ? titleTemplate : throw new ArgumentException("TitleTemplate cannot be null or empty");
            BodyTemplate = !string.IsNullOrEmpty(bodyTemplate) ? bodyTemplate : throw new ArgumentException("BodyTemplate cannot be null or empty");
            DefaultMetadata = defaultMetadata ?? DefaultMetadata;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
