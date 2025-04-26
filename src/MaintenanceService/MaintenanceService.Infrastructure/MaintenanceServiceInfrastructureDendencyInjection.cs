using MaintenanceService.Domain.Repositories;
using MaintenanceService.Infrastructure.Data;
using MaintenanceService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MaintenanceService.Infrastructure
{
    public static class MaintenanceServiceInfrastructureDependencyInjection
    {
        public static IServiceCollection AddMaintenanceServiceInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<MaintenanceDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("MaintenanceConnection")));

            services.AddScoped<IMaintenanceRepository, MaintenanceRepository>();
            return services;
        }
    }
}
