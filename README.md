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

# Aproximación intuitiva
## 1. La Base de datos con Docker (compose.yml)
Con Docker se crea un contenedor prefabricado, si se rompe o se requiere usar en otro dispositivo para desarrollo es más sencillo.
Así se logra que funcione en diferentes entornos. Simulando un restaruante es el local que se alquiladonde montaremos todo, los clientes, mesas, etc.

## 2. La API: El Mesero (VehiclesController.cs)
Capa: Presentación (AutoFleet.API).

¿Qué hace?

Recibe al cliente (Postman/React).

Toma la orden (POST /api/vehicles).

¡OJO! No cocina. El mesero no toca la sartén. Solo pasa la nota a la cocina.

¿Por qué está ahí?

Separa la "puerta de entrada" de la lógica. Si mañana quieres cambiar la API por una App de Consola o una App Móvil, la lógica de cocina no cambia, solo cambias al mesero.

## 3. La Application: El Chef (VehicleService.cs)
Capa: Aplicación (AutoFleet.Application).

¿Qué hace?

Recibe la nota del mesero.

Revisa las reglas: "¿Tenemos ingredientes? ¿El cliente es alérgico?". Aquí van las validaciones de negocio (ej: "No aceptamos autos anteriores a 1900").

Coordina. Le pide al almacén los ingredientes y prepara el plato final (el DTO).

Los DTOs (VehicleDto.cs):

Es el Menú. El cliente elige del menú, no entra a la despensa a morder una vaca cruda.

Tu lección: Nunca expongas tu Entidad de base de datos (la vaca) directamente al cliente. Usa DTOs (la hamburguesa) para proteger tus datos internos.

## 4. El Core: Las Leyes de la Física (Vehicle.cs y Interfaces)
Capa: Dominio (AutoFleet.Core).

¿Qué hace?

Define QUÉ es un vehículo. Un vehículo tiene marca y modelo aquí y en China. No le importa si se guarda en SQL, en Excel o en una servilleta.

Define las Interfaces (IVehicleRepository): Son los "Contratos". El Core dice: "Necesito alguien que sepa guardar vehículos, no me importa cómo lo haga, solo firmen aquí".

¿Por qué está ahí?

Es el corazón puro. No tiene dependencias externas (ni NuGet de SQL, ni de API). Es lo más estable de tu sistema.

## 5. La Infrastructure: El Almacén (VehicleRepository.cs y DbContext)
Capa: Infraestructura (AutoFleet.Infrastructure).

¿Qué hace?

Es el único que sabe que estamos usando SQL Server.

Implementa el contrato del Core. El Chef (Service) le dice "Guárdame esto" y el Almacén (Repository) sabe cómo traducir eso a INSERT INTO Vehicles....

La Inyección de Dependencias (DependencyInjection.cs):

Es el momento en que el dueño del restaurante conecta todo. Dice: "Cuando el Chef pida un almacén, denle ESTE almacén de SQL Server".

Tu lección: Esto permite cambiar SQL Server por MongoDB en el futuro tocando solo esta capa, sin romper al Chef ni al Mesero.

# Agregando feature/002 una BD adicional para Persistencia políglota
dotnet add AutoFleet.Infrastructure/AutoFleet.Infrastructure.csproj package MongoDB.Driver

# Agregando feature/003 Optimizador de Flota para Eventos
## Planteamiento
El Problema de Negocio:
Imagina que AutoFleet tiene un cliente corporativo (digamos, una empresa que organiza retiros). Te llaman y dicen:
"Tengo que transportar a 87 empleados al aeropuerto pero en el futuro serán N. ¿Cuál es la mínima cantidad de vehículos que debo alquilar para llevarlos a todos, optimizando costos (usando los vehículos más grandes primero)?"
El Mapeo del Algoritmo (Coin Change -> Fleet Allocation):
Monto Total (N): Número total de pasajeros (ej. 87).
Monedas: Capacidad de tus vehículos disponibles.
Autobús: 50 pasajeros.
Van Ejecutiva: 15 pasajeros.
SUV: 5 pasajeros.
Sedán: 4 pasajeros.
Objetivo: MinCoins -> Mínimo número de choferes/vehículos requeridos.

## Se agrega posterior a los cambios, en la migración:
dotnet ef migrations add AddCapacityAndStatus --project AutoFleet.Infrastructure --startup-project AutoFleet.API
dotnet ef database update --project AutoFleet.Infrastructure --startup-project AutoFleet.API

# Agregando una primera opción de autenticación en feature/004 aprovechando que se unifican solucion inicial y problemática
dotnet add AutoFleet.API/AutoFleet.API.csproj package Microsoft.AspNetCore.Authentication.JwtBearer
## Creando a El Cliente Simulador (Consumo + Polly)
### En la raíz
dotnet new console -n AutoFleet.ConsoleClient
dotnet sln add AutoFleet.ConsoleClient/AutoFleet.ConsoleClient.csproj

### Agregar paquetes de HTTP y Polly
dotnet add AutoFleet.ConsoleClient/AutoFleet.ConsoleClient.csproj package Microsoft.Extensions.Http
dotnet add AutoFleet.ConsoleClient/AutoFleet.ConsoleClient.csproj package Microsoft.Extensions.Http.Polly
dotnet add AutoFleet.ConsoleClient/AutoFleet.ConsoleClient.csproj package Newtonsoft.Json

# Agregando tests
## 1. Crear proyecto xUnit
dotnet new xunit -n AutoFleet.Tests

## 2. Agregarlo a la solución
dotnet sln add AutoFleet.Tests/AutoFleet.Tests.csproj

## 3. Referencias: El test necesita ver a Application y Core
dotnet add AutoFleet.Tests/AutoFleet.Tests.csproj reference AutoFleet.Application/AutoFleet.Application.csproj
dotnet add AutoFleet.Tests/AutoFleet.Tests.csproj reference AutoFleet.Core/AutoFleet.Core.csproj

## 4. Instalar Moq (Para simular el Repositorio)
dotnet add AutoFleet.Tests/AutoFleet.Tests.csproj package Moq

# Logs en Application
## Agregando la interface ILogger en Application, la implementacion real se importa en API
dotnet add AutoFleet.Application/ package Microsoft.Extensions.Logging.Abstractions

# Reseteo de todo en EF
dotnet ef database drop --project AutoFleet.Infrastructure --startup-project AutoFleet.API --force
### Borrar la carpeta de migraciones completa
dotnet ef migrations add InitialCreate --project AutoFleet.Infrastructure --startup-project AutoFleet.API
dotnet ef database update --project AutoFleet.Infrastructure --startup-project AutoFleet.API

# Agregando auth correctamente y almacenando los resultados en la BD
## Instalando BCRYPT
dotnet add AutoFleet.Application/AutoFleet.Application.csproj package BCrypt.Net-Next
## Creando nueva migracion 
dotnet ef migrations add AddUsersTable
dotnet ef database update --project AutoFleet.Infrastructure --startup-project AutoFleet.API
### Nota, dotnet ef migrations script genera SQL sin ejecutarlo
## Para el token y auth requerimos agregar en la classlib:
### Para leer IConfiguration (appsettings)
dotnet add AutoFleet.Application/AutoFleet.Application.csproj package Microsoft.Extensions.Configuration.Abstractions
### Para generar los JWT (Tokens)
dotnet add AutoFleet.Application/AutoFleet.Application.csproj package System.IdentityModel.Tokens.Jwt

