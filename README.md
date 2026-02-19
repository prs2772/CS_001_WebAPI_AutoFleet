# Crear la solución vacía
dotnet new sln -n AutoFleet

# 1. Capa de API (Presentación - Controllers, Middleware, Swagger, JWT)
dotnet new webapi -n AutoFleet.API

# 2. Capa de Core (Dominio - Entidades, Interfaces, DTOs)
dotnet new classlib -n AutoFleet.Core

# 3. Capa de Infrastructure (Datos - Repositories, EF Core, Migrations)
dotnet new classlib -n AutoFleet.Infrastructure

# 4. Crear la capa de Aplicación
dotnet new classlib -n AutoFleet.Application

# Agregar proyectos a la solución
dotnet sln add AutoFleet.API/AutoFleet.API.csproj
dotnet sln add AutoFleet.Core/AutoFleet.Core.csproj
dotnet sln add AutoFleet.Infrastructure/AutoFleet.Infrastructure.csproj
dotnet sln add AutoFleet.Application/AutoFleet.Application.csproj

# Referencias entre capas (La dependencia fluye hacia adentro o hacia infraestructura)
# API usa Core e Infrastructure
dotnet add AutoFleet.API/AutoFleet.API.csproj reference AutoFleet.Core/AutoFleet.Core.csproj
dotnet add AutoFleet.API/AutoFleet.API.csproj reference AutoFleet.Infrastructure/AutoFleet.Infrastructure.csproj
dotnet add AutoFleet.API/AutoFleet.API.csproj reference AutoFleet.Application/AutoFleet.Application.csproj

# Infrastructure usa Core (para implementar interfaces)
dotnet add AutoFleet.Infrastructure/AutoFleet.Infrastructure.csproj reference AutoFleet.Core/AutoFleet.Core.csproj
dotnet add AutoFleet.Infrastructure/AutoFleet.Infrastructure.csproj reference AutoFleet.Application/AutoFleet.Application.csproj

# Application usa Core (porque maneja Entidades)
dotnet add AutoFleet.Application/AutoFleet.Application.csproj reference AutoFleet.Core/AutoFleet.Core.csproj

# Agrega EF
# Para el proyecto de Infraestructura (el que hace el trabajo sucio)
dotnet add AutoFleet.Infrastructure/AutoFleet.Infrastructure.csproj package Microsoft.EntityFrameworkCore.SqlServer

# Para el proyecto de API (el que ejecuta los comandos de herramientas)
dotnet add AutoFleet.API/AutoFleet.API.csproj package Microsoft.EntityFrameworkCore.Design

# Crear la migración inicial (esto genera código C# SQL)
dotnet ef migrations add InitialCreate --project AutoFleet.Infrastructure --startup-project AutoFleet.API

# Aplicar la migración (esto ejecuta el SQL en el contenedor)
dotnet ef database update --project AutoFleet.Infrastructure --startup-project AutoFleet.API

# Instalar el swagger
dotnet add AutoFleet.API/AutoFleet.API.csproj package Swashbuckle.AspNetCore
