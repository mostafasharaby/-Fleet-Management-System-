using FleetManagement.Client;

var builder = Host.CreateApplicationBuilder(args);
//builder.Services.AddHostedService<Worker>();
builder.Services.AddHostedService<VehicleClient>();

var host = builder.Build();
host.Run();
