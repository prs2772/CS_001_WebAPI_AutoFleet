using AutoFleet.Application.Interfaces;
using AutoFleet.Application.Services;
using AutoFleet.Core.Interfaces;
using AutoFleet.Infrastructure.Data;
using AutoFleet.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AutoFleet.API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        // 1. Database Configuration
        services.AddDbContext<AutoFleetDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        // 2. CORS Configuration
        // Allowing any origin for development flexibility
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader());
        });

        // 3. Dependency Injection (DI) Container

        // Registering SQL Server Repository (Default implementation)
        services.AddScoped<IVehicleRepository, VehicleRepository>();

        // Registering Mongo DB Repository (Secondary implementation)
        // Note: In .NET DI, when resolving IEnumerable<IVehicleRepository>, both will be injected.
        // When resolving single IVehicleRepository, the LAST one registered wins (unless handled manually).
        services.AddScoped<IVehicleRepository, MongoVehicleRepository>();

        // Registering Users on SQL Repository
        services.AddScoped<IUserRepository, UserRepository>();

        // Domain Services
        services.AddScoped<IVehicleService, VehicleService>();
        services.AddScoped<IFleetOptimizerService, FleetOptimizerService>();

        // 4. Swagger / API Explorer
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }
}
