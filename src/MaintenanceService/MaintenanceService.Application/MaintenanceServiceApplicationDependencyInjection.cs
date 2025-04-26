using MaintenanceService.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MaintenanceService.Application
{
    public static class MaintenanceServiceApplicationDependencyInjection
    {
        public static IServiceCollection AddMaintenancServiceApplication(this IServiceCollection services)
        {
            services.AddScoped<IMaintenanceService, Services.MaintenanceService>();
            return services;
        }
    }
}
