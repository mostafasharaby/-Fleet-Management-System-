using TelemetryService.API.Services;
using TelemetryService.Application;
using TelemetryService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc(option => option.EnableDetailedErrors = true);
builder.Services.AddGrpcReflection();

builder.Services.AddTelemetryServiceApplication();
builder.Services.AddTelemetryServiceInfrastructure(builder.Configuration);

builder.Services.AddControllers();
var app = builder.Build();


app.UseRouting();

app.UseEndpoints(endpoint =>
{
    endpoint.MapGrpcService<TelemetryGrpcService>();
    if (app.Environment.IsDevelopment())
    {
        endpoint.MapGrpcReflectionService();
    }
    endpoint.MapControllers();
});

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
