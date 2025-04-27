namespace NotificationService.Application.Interfaces
{
    public interface ITemplateProcessor
    {
        Task<(string Title, string Body)> ProcessTemplateAsync(string templateId, Dictionary<string, string> templateData);
    }
}
