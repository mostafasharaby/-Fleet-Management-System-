using FleetManagement.Client;
using FleetManagement.Client.Services;

var builder = Host.CreateApplicationBuilder(args);
//builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<VehicleServiceClient>(provider =>
{
    var logger = provider.GetRequiredService<ILogger<VehicleServiceClient>>();
    string serviceUrl = "https://localhost:7206";
    return new VehicleServiceClient(serviceUrl, logger);
});
builder.Services.AddHostedService<Worker>();
var host = builder.Build();
host.Run();
