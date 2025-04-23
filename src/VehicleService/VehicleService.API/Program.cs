using VehicleService.API.Services;
using VehicleService.Application;
using VehicleService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc(option =>
{
    option.EnableDetailedErrors = true;
});
builder.Services.AddGrpcReflection();

builder.Services.AddVehicleServiceApplication();
builder.Services.AddVehicleServiceInfrastructure(builder.Configuration);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GrpcVehicleService>();

app.UseRouting(); // Matches request to an endpoint.
app.UseEndpoints(endpoint =>  //Execute the matched endpoint.
{
    endpoint.MapGrpcService<GrpcVehicleService>();
    if (app.Environment.IsDevelopment())
    {
        endpoint.MapGrpcReflectionService();
    }
    endpoint.MapControllers();
});

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
