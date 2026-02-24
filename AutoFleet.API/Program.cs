using AutoFleet.API.Extensions;
using AutoFleet.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// 1. Service Registration (The "What")

builder.Services.AddControllers();

// Passing 'builder.Configuration' allows extensions to read appsettings.json
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

// 2. Pipeline Configuration (The "How")

var app = builder.Build();

// Global Error Handling Middleware (Must be at the top)
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection(); // Uncomment for Production

// CORS must be before Auth
app.UseCors("AllowAll");

// Security Pipeline
app.UseAuthentication(); // Who are you?
app.UseAuthorization();  // What can you do?

// Endpoint Mapping
app.MapControllers();

app.Run();
