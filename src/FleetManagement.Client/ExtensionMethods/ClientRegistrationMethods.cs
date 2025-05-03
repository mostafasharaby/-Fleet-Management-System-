using AutoMapper;
using FleetManagement.Client.Services;
using FleetManagement.Client.Workers;

namespace FleetManagement.Client.ExtensionMethods
{
    public static class ClientRegistrationMethods
    {
        public static IServiceCollection AddClientExtensionMethods(this IServiceCollection services)
        {
            services.AddSingleton<VehicleServiceClient>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<VehicleServiceClient>>();
                string serviceUrl = "https://localhost:7206";
                return new VehicleServiceClient(serviceUrl, logger);
            });
            services.AddSingleton<DriverServiceClient>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<DriverServiceClient>>();
                string serviceUrl = "https://localhost:7240";
                return new DriverServiceClient(serviceUrl, logger);
            });

            services.AddSingleton<RouteServiceClient>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<RouteServiceClient>>();
                string serviceUrl = "https://localhost:7183";
                var mapper = provider.GetRequiredService<IMapper>(); //resolve mapper from DI
                return new RouteServiceClient(serviceUrl, logger, mapper);
            });


            services.AddHostedService<DriverWorker>();
            services.AddHostedService<VehicleWorker>();
            services.AddHostedService<RouteWorker>();

            return services;
        }
    }
}
