using Auth.Api.Services;
using Auth.Application;
using Auth.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services.AddAuthInfrastructure(builder.Configuration);
builder.Services.AddAuthApplication();
builder.Services.ConfigureApplicationCookie(options =>  // 	Configure cookies to return 401/403 instead of 302
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
});




builder.Services.AddControllers();
var app = builder.Build();
app.UseRouting();

app.UseCors("MyPolicy");
//app.MapGrpcService<AuthGrpcService>();
//app.MapGrpcService<RoleGrpcService>();
//app.MapGrpcService<ClaimsGrpcService>();



app.UseAuthentication();
app.UseAuthorization();
app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });

app.UseEndpoints(endpoint =>
{
    endpoint.MapGrpcService<AuthGrpcService>().EnableGrpcWeb();
    //endpoint.MapGrpcService<RoleGrpcService>();
    //endpoint.MapGrpcService<ClaimsGrpcService>();
    if (app.Environment.IsDevelopment())
    {
        endpoint.MapGrpcReflectionService();
    }
    endpoint.MapControllers();
});

app.Run();
