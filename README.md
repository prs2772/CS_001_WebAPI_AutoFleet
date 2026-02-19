# Esqueleto de la aplicación - ESQ

## ESQ.1 Crear la solución y las capas
dotnet new sln -n AutoFleet
## ESQ.2 Crear cada capa
### ESQ.2.1 Capa de API (Presentación - Controllers, Middleware, Swagger, JWT)
dotnet new webapi -n AutoFleet.API
### ESQ.2.2 Capa de Core (Dominio - Entidades, Interfaces, DTOs)
dotnet new classlib -n AutoFleet.Core
### ESQ.2.3 Capa de Infrastructure (Datos - Repositories, EF Core, Migrations)
dotnet new classlib -n AutoFleet.Infrastructure
### ESQ.2.4 Crear la capa de Aplicación (Casos de uso)
dotnet new classlib -n AutoFleet.Application

## ESQ.3 Agregar proyectos a la solución
dotnet sln add AutoFleet.API/AutoFleet.API.csproj
dotnet sln add AutoFleet.Core/AutoFleet.Core.csproj
dotnet sln add AutoFleet.Infrastructure/AutoFleet.Infrastructure.csproj
dotnet sln add AutoFleet.Application/AutoFleet.Application.csproj

## ESQ.4 Referencias entre capas (La dependencia fluye hacia adentro o hacia infraestructura)
### ESQ.4.1 API usa Core e Infrastructure
dotnet add AutoFleet.API/AutoFleet.API.csproj reference AutoFleet.Core/AutoFleet.Core.csproj
dotnet add AutoFleet.API/AutoFleet.API.csproj reference AutoFleet.Infrastructure/AutoFleet.Infrastructure.csproj
dotnet add AutoFleet.API/AutoFleet.API.csproj reference AutoFleet.Application/AutoFleet.Application.csproj
#### Se agrega a API Infraestructure
dotnet add AutoFleet.API reference AutoFleet.Infrastructure/
### ESQ.4.2 Infrastructure usa Core (para implementar interfaces)
dotnet add AutoFleet.Infrastructure/AutoFleet.Infrastructure.csproj reference AutoFleet.Core/AutoFleet.Core.csproj
dotnet add AutoFleet.Infrastructure/AutoFleet.Infrastructure.csproj reference AutoFleet.Application/AutoFleet.Application.csproj
### ESQ.4.3 Application usa Core (porque maneja Entidades)
dotnet add AutoFleet.Application/AutoFleet.Application.csproj reference AutoFleet.Core/AutoFleet.Core.csproj

## ESQ.5 Agrega EF
### ESQ.5.1 Para el proyecto de Infraestructura (el que hace el trabajo sucio)
dotnet add AutoFleet.Infrastructure/AutoFleet.Infrastructure.csproj package Microsoft.EntityFrameworkCore.SqlServer
### ESQ.5.2 Para el proyecto de API (el que ejecuta los comandos de herramientas)
dotnet add AutoFleet.API/AutoFleet.API.csproj package Microsoft.EntityFrameworkCore.Design

## ESQ.6 Crear la migración inicial (esto genera código C# SQL)
dotnet ef migrations add InitialCreate --project AutoFleet.Infrastructure --startup-project AutoFleet.API

## ESQ.7 Aplicar la migración (esto ejecuta el SQL en el contenedor)
dotnet ef database update --project AutoFleet.Infrastructure --startup-project AutoFleet.API

## ESQ.8 Instalar el swagger
dotnet add AutoFleet.API/AutoFleet.API.csproj package Swashbuckle.AspNetCore
