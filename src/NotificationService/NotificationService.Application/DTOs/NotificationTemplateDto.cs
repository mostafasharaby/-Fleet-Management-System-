using NotificationService.Domain.Enums;

namespace NotificationService.Application.DTOs
{
    public class NotificationTemplateDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public NotificationType Type { get; set; }
        public string TitleTemplate { get; set; }
        public string BodyTemplate { get; set; }
        public Dictionary<string, string> DefaultMetadata { get; set; }
    }

    public record NotificationCreationResponse(bool Success, string TempleteId, string ErrorMessage);

}
