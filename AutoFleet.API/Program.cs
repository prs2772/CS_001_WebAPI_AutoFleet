using AutoFleet.Infrastructure.Data;
using AutoFleet.Infrastructure.Repositories;
using AutoFleet.Core.Interfaces;
using AutoFleet.Application.Interfaces;
using AutoFleet.Application.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar DB Context
builder.Services.AddDbContext<AutoFleetDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Inyección de Dependencias (DI)
// "Cuando alguien pida IVehicleRepository, dale un VehicleRepository"
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IFleetOptimizerService, FleetOptimizerService>();

// 3. Registramos Mongo Repository (¡NUEVO!)
// Al usar AddScoped de nuevo con la misma interfaz, .NET crea una colección.
builder.Services.AddScoped<IVehicleRepository, MongoVehicleRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuración de JWT
var key = Encoding.ASCII.GetBytes("EstaEsMiClaveSecretaSuperSeguraParaAutoFleet2026!"); // En PROD va a appsettings o KeyVault
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// Habilitar CORS (Para que React/Angular/Consola puedan entrar)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

app.UseMiddleware<AutoFleet.API.Middlewares.ExceptionMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll"); // <--- Importante para que el frontend pueda consumir la API sin problemas de CORS
// app.UseHttpsRedirection();
app.UseAuthentication(); // <--- OBLIGATORIO: ¿Quién eres?
app.UseAuthorization();  // <--- OBLIGATORIO: ¿Qué puedes hacer?
app.MapControllers();

app.Run();
