using DriverService.Domain.Repositories;
using DriverService.Infrastructure.Data;
using DriverService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DriverService.Infrastructure
{
    public static class DriverServiceinfrastructureDependencyInjection
    {
        public static IServiceCollection AddDriverServiceInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContext<DriverDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DriverConnection")));

            services.AddScoped<IDriverRepository, DriverRepository>();
            return services;
        }
    }
}
