using FleetManagement.Client.ExtensionMethods;
using System.Reflection;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddClientExtensionMethods();
var host = builder.Build();
host.Run();
