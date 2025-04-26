using MaintenanceService.API.Services;
using MaintenanceService.Application;
using MaintenanceService.Infrastructure;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc(option => option.EnableDetailedErrors = true);
builder.Services.AddGrpcReflection();

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddMaintenanceServiceInfrastructure(builder.Configuration);
builder.Services.AddMaintenancServiceApplication();
builder.Services.AddControllers();
var app = builder.Build();

app.UseRouting();
app.UseEndpoints(endpoint =>
{
    endpoint.MapGrpcService<GrpcMaintenanceService>();
    if (app.Environment.IsDevelopment())
    {
        endpoint.MapGrpcReflectionService();
    }
    endpoint.MapControllers();

});
app.Run();
