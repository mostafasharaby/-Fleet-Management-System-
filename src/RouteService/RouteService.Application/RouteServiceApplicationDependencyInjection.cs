using Microsoft.Extensions.DependencyInjection;
using RouteService.Application.Services;
using RouteService.Domain.Services;
using RouteService.Infrastructure.Services;

namespace RouteService.Application
{
    public static class RouteServiceApplicationDependencyInjection
    {
        public static IServiceCollection AddRouteServiceApplication(this IServiceCollection services)
        {
            services.AddScoped<IRouteService, Services.RouteService>();
            services.AddScoped<IRouteOptimizationService, RouteOptimizationService>();
            return services;
        }
    }
}
