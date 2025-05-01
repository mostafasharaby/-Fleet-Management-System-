using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TelemetryService.Domain.Messaging;
using TelemetryService.Domain.Repositories;
using TelemetryService.Infrastructure.Caching;
using TelemetryService.Infrastructure.Data;
using TelemetryService.Infrastructure.Messaging;
using TelemetryService.Infrastructure.Repositories;

namespace TelemetryService.Infrastructure
{
    public static class TelemetryServiceInfrastructureDependencyInjection
    {
        public static IServiceCollection AddTelemetryServiceInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContext<TelemetryDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("TelemetryConnection")));

            services.AddScoped<ITelemetryRepository, TelemetryRepository>();
            services.AddScoped<IAlertThresholdRepository, AlertThresholdRepository>();
            services.AddScoped<ITelemetryCache, TelemetryCache>();
            services.AddScoped<IMessagePublisher, RabbitMQPublisher>();
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();
            return services;
        }
    }
}
