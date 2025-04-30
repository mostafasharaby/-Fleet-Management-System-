using Auth.Api.Services;
using Auth.Application;
using Auth.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services.AddAuthInfrastructure(builder.Configuration);
builder.Services.AddAuthApplication();

builder.Services.AddControllers();

var app = builder.Build();

//app.MapGrpcService<AuthGrpcService>();
//app.MapGrpcService<RoleGrpcService>();
//app.MapGrpcService<ClaimsGrpcService>();


app.UseRouting();
app.UseEndpoints(endpoint =>
{
    endpoint.MapGrpcService<AuthGrpcService>();
    //endpoint.MapGrpcService<RoleGrpcService>();
    //endpoint.MapGrpcService<ClaimsGrpcService>();
    if (app.Environment.IsDevelopment())
    {
        endpoint.MapGrpcReflectionService();
    }
    endpoint.MapControllers();
});

app.Run();
