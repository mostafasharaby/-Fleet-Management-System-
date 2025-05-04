using AutoMapper;
using FleetManagement.Client.Services;
using FleetManagement.Client.Workers;

namespace FleetManagement.Client.ExtensionMethods
{
    public static class ClientRegistrationMethods
    {
        public static IServiceCollection AddClientExtensionMethods(this IServiceCollection services)
        {
            services.AddSingleton(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<VehicleServiceClient>>();
                string serviceUrl = "https://localhost:7206";
                return new VehicleServiceClient(serviceUrl, logger);
            });

            services.AddSingleton(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<DriverServiceClient>>();
                string serviceUrl = "https://localhost:7240";
                return new DriverServiceClient(serviceUrl, logger);
            });

            services.AddSingleton(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<RouteServiceClient>>();
                string serviceUrl = "https://localhost:7183";
                var mapper = provider.GetRequiredService<IMapper>(); //resolve mapper from DI
                return new RouteServiceClient(serviceUrl, logger, mapper);
            });

            services.AddSingleton(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<MaintenanceServiceClient>>();
                string serviceUrl = "https://localhost:7292";
                var mapper = provider.GetRequiredService<IMapper>();
                return new MaintenanceServiceClient(serviceUrl, logger, mapper);
            });

            services.AddSingleton(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<TelemetryServiceClient>>();
                string serviceUrl = "https://localhost:7280";
                return new TelemetryServiceClient(serviceUrl, logger);
            });

            services.AddSingleton(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<NotificationServiceClient>>();
                string serviceUrl = "https://localhost:7171";
                var mapper = provider.GetRequiredService<IMapper>();
                return new NotificationServiceClient(serviceUrl, logger, mapper);
            });


            services.AddHostedService<DriverWorker>();
            services.AddHostedService<VehicleWorker>();
            services.AddHostedService<RouteWorker>();
            services.AddHostedService<MaintenanceWorker>();
            services.AddHostedService<TelemetryWorker>();
            services.AddHostedService<NotificationWorker>();

            return services;
        }
    }
}
