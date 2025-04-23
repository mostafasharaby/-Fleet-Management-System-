using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VehicleService.Domain.Repositories;
using VehicleService.Infrastructure.Data;
using VehicleService.Infrastructure.Repositories;

namespace VehicleService.Infrastructure
{
    public static class VehicleServiceInfrastructureDependencyInjection
    {
        public static IServiceCollection AddVehicleServiceInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<VehicleDbContext>(options =>
                         options.UseSqlServer(configuration.GetConnectionString("VehicleConnection")));

            services.AddScoped<IVehicleRepository, VehicleRepository>();
            return services;
        }
    }
}
