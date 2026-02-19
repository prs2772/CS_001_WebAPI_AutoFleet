using AutoFleet.Infrastructure.Data;
using AutoFleet.Infrastructure.Repositories;
using AutoFleet.Core.Interfaces;
using AutoFleet.Application.Interfaces; // Asegúrate de tener este using
using AutoFleet.Application.Services;   // Y este
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar DB Context
builder.Services.AddDbContext<AutoFleetDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Inyección de Dependencias (DI)
// "Cuando alguien pida IVehicleRepository, dale un VehicleRepository"
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

// 3. Registramos Mongo Repository (¡NUEVO!)
// Al usar AddScoped de nuevo con la misma interfaz, .NET crea una colección.
builder.Services.AddScoped<IVehicleRepository, MongoVehicleRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
