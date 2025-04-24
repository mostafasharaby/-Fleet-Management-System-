using DriverService.API.Services;
using DriverService.Application;
using DriverService.Infrastructure;
using System.Reflection;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc(option => option.EnableDetailedErrors = true);
builder.Services.AddGrpcReflection();

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddDriverServiceInfrastructure(builder.Configuration);
builder.Services.AddDriverServiceApplication();

builder.Services.AddControllers();
var app = builder.Build();


app.UseRouting();
app.UseEndpoints(endpoint =>
{
    endpoint.MapGrpcService<GrpcDriverService>();
    if (app.Environment.IsDevelopment())
    {
        endpoint.MapGrpcReflectionService();
    }
    endpoint.MapControllers();
});

app.Run();
