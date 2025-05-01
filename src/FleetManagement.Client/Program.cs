using FleetManagement.Client.Services;
using FleetManagement.Client.Workers;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<VehicleServiceClient>(provider =>
{
    var logger = provider.GetRequiredService<ILogger<VehicleServiceClient>>();
    string serviceUrl = "https://localhost:7206";
    return new VehicleServiceClient(serviceUrl, logger);
});
builder.Services.AddSingleton<DriverServiceClient>(provider =>
{
    var logger = provider.GetRequiredService<ILogger<DriverServiceClient>>();
    string serviceUrl = "https://localhost:7206";
    return new DriverServiceClient(serviceUrl, logger);
});

builder.Services.AddHostedService<DriverWorker>();
builder.Services.AddHostedService<VehicleWorker>();

var host = builder.Build();
host.Run();
