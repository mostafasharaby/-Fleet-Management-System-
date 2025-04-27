using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TelemetryService.Application.Services;

namespace TelemetryService.Application
{
    public static class TelemetryServiceApplicationDependencyInjection
    {
        public static IServiceCollection AddTelemetryServiceApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddScoped<ITelemetryService, Services.TelemetryService>();
            services.AddScoped<IAlertThresholdService, AlertThresholdService>();
            return services;
        }
    }
}
