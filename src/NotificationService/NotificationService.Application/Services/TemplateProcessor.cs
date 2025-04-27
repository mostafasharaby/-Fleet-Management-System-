using NotificationService.Application.Interfaces;
using NotificationService.Domain.Repositories;

namespace NotificationService.Application.Services
{
    public class TemplateProcessor : ITemplateProcessor
    {
        private readonly INotificationTemplateRepository _templateRepository;

        public TemplateProcessor(INotificationTemplateRepository templateRepository)
        {
            _templateRepository = templateRepository ?? throw new ArgumentNullException(nameof(templateRepository));
        }

        public async Task<(string Title, string Body)> ProcessTemplateAsync(string templateId, Dictionary<string, string> templateData)
        {
            var template = await _templateRepository.GetByIdAsync(templateId);
            if (template == null)
            {
                throw new ArgumentException($"Template with ID {templateId} not found");
            }

            var title = ProcessTemplateText(template.TitleTemplate, templateData);
            var body = ProcessTemplateText(template.BodyTemplate, templateData);

            return (title, body);
        }

        private string ProcessTemplateText(string templateText, Dictionary<string, string> templateData)
        {
            if (templateData == null || !templateData.Any())
            {
                return templateText;
            }

            var result = templateText;
            foreach (var kvp in templateData)
            {
                result = result.Replace($"{{{{{kvp.Key}}}}}", kvp.Value);
            }

            return result;
        }
    }
}
