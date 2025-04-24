﻿using DriverService.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DriverService.Application
{
    public static class DriverServiceApplicationDependencyInjection
    {
        public static IServiceCollection AddDriverServiceApplication(this IServiceCollection services)
        {
            services.AddScoped<IDriverService, Services.DriverService>();
            return services;
        }
    }
}
