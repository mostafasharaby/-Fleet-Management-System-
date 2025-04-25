using RouteService.API.Services;
using RouteService.Application;
using RouteService.Infrastructure;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc(option => option.EnableDetailedErrors = true);
builder.Services.AddGrpcReflection();

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddRouteServiceInfrastructure(builder.Configuration);
builder.Services.AddRouteServiceApplication();
builder.Services.AddControllers();

var app = builder.Build();


app.UseRouting();
app.UseEndpoints(endpoint =>
{
    endpoint.MapGrpcService<GrpcRouteService>();
    if (app.Environment.IsDevelopment())
    {
        endpoint.MapGrpcReflectionService();
    }
    endpoint.MapControllers();
});
app.Run();
