using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration; // Para IConfiguration
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models; // <--- ESTE ES EL CLAVE
using System.Reflection;
using AutoFleet.Infrastructure.Data;
using AutoFleet.Infrastructure.Repositories;
using AutoFleet.Application.Interfaces;
using AutoFleet.Application.Services;
using AutoFleet.Core.Interfaces;

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
        // services.AddScoped<IVehicleRepository, MongoVehicleRepository>();

        // Registering Users on SQL Repository
        services.AddScoped<IUserRepository, UserRepository>();

        // Domain Services
        services.AddScoped<IVehicleService, VehicleService>();
        services.AddScoped<IFleetOptimizerService, FleetOptimizerService>();
        services.AddScoped<IAuthService, AuthService>();

        // 4. Swagger / API Explorer
        services.AddEndpointsApiExplorer();
        // 4. Swagger con Documentación y Seguridad JWT
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "AutoFleet API",
                Version = "v1",
                Description = "API para gestión y optimización de flotas vehiculares."
            });

            // Configuración del botón "Authorize" (Candadito)
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
            });

            // Habilitar comentarios XML (Para que se vea la documentación que escribiste en el Controller)
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });

        return services;
    }
}
