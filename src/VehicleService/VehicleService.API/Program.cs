using Microsoft.EntityFrameworkCore;
using VehicleService.API.Services;
using VehicleService.Application.Services;
using VehicleService.Domain.Repositories;
using VehicleService.Infrastructure.Data;
using VehicleService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddGrpc(option =>
{
    option.EnableDetailedErrors = true;
});

// Configure database
builder.Services.AddDbContext<VehicleDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("VehicleConnection")));

// Configure application services
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IVehicleService, VehicleService.Application.Services.VehicleService>();

// Add health checks
//builder.Services.AddHealthChecks()
//    .AddDbContextCheck<VehicleDbContext>();

//// Add OpenTelemetry
//builder.Services.AddOpenTelemetry()
//    .WithTracing(tracerProviderBuilder =>
//    {
//        tracerProviderBuilder
//            .AddSource("FleetManagement.VehicleService")
//            .AddGrpcClientInstrumentation()
//            .AddAspNetCoreInstrumentation()
//            .AddEntityFrameworkCoreInstrumentation()
//            .AddOtlpExporter(options => options.Endpoint = new Uri(builder.Configuration["Telemetry:OtlpEndpoint"]));
//    })
//    .WithMetrics(meterProviderBuilder =>
//    {
//        meterProviderBuilder
//            .AddMeter("FleetManagement.VehicleService")
//            .AddAspNetCoreInstrumentation()
//            .AddOtlpExporter(options => options.Endpoint = new Uri(builder.Configuration["Telemetry:OtlpEndpoint"]));
//    });

var app = builder.Build();
app.MapGrpcService<GrpcVehicleService>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
