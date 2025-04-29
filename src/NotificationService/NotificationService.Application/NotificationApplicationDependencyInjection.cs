using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Interfaces;
using NotificationService.Application.Services;

namespace NotificationService.Application
{
    public static class NotificationApplicationDependencyInjection
    {
        public static IServiceCollection AddNotificationApplication(this IServiceCollection services)
        {
            services.AddScoped<INotificationService, NotificationAppService>();
            services.AddScoped<ITemplateProcessor, TemplateProcessor>();
            services.AddScoped<NotificationChannelFactory>();
            return services;
        }
    }
}
