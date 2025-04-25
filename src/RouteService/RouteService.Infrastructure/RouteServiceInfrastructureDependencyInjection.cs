using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RouteService.Domain.Repositories;
using RouteService.Infrastructure.Data;
using RouteService.Infrastructure.Repositories;

namespace RouteService.Infrastructure
{
    public static class RouteServiceInfrastructureDependencyInjection
    {
        public static IServiceCollection AddRouteServiceInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<RouteDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("RouteConnection")));

            services.AddScoped<IRouteRepository, RouteRepository>();
            services.AddScoped<IRouteStopRepository, RouteStopRepository>();
            return services;
        }
    }
}
