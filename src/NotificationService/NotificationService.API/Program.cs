using NotificationService.API.Services;
using NotificationService.Application;
using NotificationService.Infrastructure;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddGrpc(option => option.EnableDetailedErrors = true);
builder.Services.AddGrpcReflection();

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddNotificationInfrastructure(builder.Configuration);
builder.Services.AddNotificationApplication();
builder.Services.AddControllers();

var app = builder.Build();

app.UseGrpcWeb();


app.UseRouting();
app.UseEndpoints(endpoint =>
{
    endpoint.MapGrpcService<NotificationGrpcService>().EnableGrpcWeb();
    if (app.Environment.IsDevelopment())
    {
        endpoint.MapGrpcReflectionService();
    }
    endpoint.MapControllers();
});
app.Run();

/*
  when I say "Program.cs can reference Infrastructure for wiring",
I mean:
➔ It references Infrastructure and uses its extension methods
➔ but only in Program.cs (and not anywhere else).
*/