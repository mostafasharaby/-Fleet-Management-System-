using Microsoft.Extensions.DependencyInjection;
using VehicleService.Application.Services;

namespace VehicleService.Application
{
    public static class VehicleServiceApplicationDependencyInjection
    {
        public static IServiceCollection AddVehicleServiceApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(VehicleServiceApplicationDependencyInjection).Assembly);
            services.AddScoped<IVehicleService, Services.VehicleService>();
            return services;
        }
    }
}
