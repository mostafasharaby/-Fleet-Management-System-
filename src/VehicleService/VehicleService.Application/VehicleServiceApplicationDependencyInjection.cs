using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using VehicleService.Application.Services;

namespace VehicleService.Application
{
    public static class VehicleServiceApplicationDependencyInjection
    {
        public static IServiceCollection AddVehicleServiceApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddScoped<IVehicleService, Services.VehicleService>();
            return services;
        }
    }
}
