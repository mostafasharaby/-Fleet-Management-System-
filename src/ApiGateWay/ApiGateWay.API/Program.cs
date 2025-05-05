using ApiGateWay.API.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Services.AddOcelot(builder.Configuration) // with circuit breaker
    .AddCacheManager(settings => settings.WithDictionaryHandle());

builder.Services.AddHealthChecks()
    .AddCheck<VehicleServiceHealthCheck>("VehicleService")
    .AddCheck<DriverServiceHealthCheck>("DriverService")
    .AddCheck<RouteServiceHealthCheck>("RouteService")
    .AddCheck<AuthServiceHealthCheck>("AuthService")
    .AddCheck<TelemetryServiceHealthCheck>("TelemetryService")
    .AddCheck<MaintenanceServiceHealthCheck>("MaintenanceService");


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

// Custom health check response
app.UseHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = report.Status.ToString(),
            components = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                duration = e.Value.Duration.TotalMilliseconds
            })
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseOcelot().Wait();

app.Run();
