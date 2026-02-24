# 📘 Documentación Ultimate - AutoFleet
# 1. Esqueleto y Creación del Proyecto
Comandos para generar la estructura Clean Architecture desde cero.

## 1.1 Crear la Solución y Capas
### Solución vacía
dotnet new sln -n AutoFleet

### Capas (Proyectos)
dotnet new webapi -n AutoFleet.API                # Presentación
dotnet new classlib -n AutoFleet.Core             # Dominio Puro
dotnet new classlib -n AutoFleet.Infrastructure   # Acceso a Datos
dotnet new classlib -n AutoFleet.Application      # Casos de Uso

### Agregar proyectos a la solución (.sln)
dotnet sln add AutoFleet.API/AutoFleet.API.csproj
dotnet sln add AutoFleet.Core/AutoFleet.Core.csproj
dotnet sln add AutoFleet.Infrastructure/AutoFleet.Infrastructure.csproj
dotnet sln add AutoFleet.Application/AutoFleet.Application.csproj

## 1.2 Referencias entre Capas (Dependencias)
La regla de oro: Las dependencias apuntan hacia adentro (Core) o hacia Infraestructura desde API.

### API conoce a todos para poder inyectarlos
dotnet add AutoFleet.API/AutoFleet.API.csproj reference AutoFleet.Core/AutoFleet.Core.csproj
dotnet add AutoFleet.API/AutoFleet.API.csproj reference AutoFleet.Infrastructure/AutoFleet.Infrastructure.csproj
dotnet add AutoFleet.API/AutoFleet.API.csproj reference AutoFleet.Application/AutoFleet.Application.csproj

### Infrastructure implementa interfaces de Core y usa DTOs de Application
dotnet add AutoFleet.Infrastructure/AutoFleet.Infrastructure.csproj reference AutoFleet.Core/AutoFleet.Core.csproj
dotnet add AutoFleet.Infrastructure/AutoFleet.Infrastructure.csproj reference AutoFleet.Application/AutoFleet.Application.csproj

### Application usa Entidades de Core
dotnet add AutoFleet.Application/AutoFleet.Application.csproj reference AutoFleet.Core/AutoFleet.Core.csproj

# 2. Configuración de Base de Datos y ORM
## 2.1 Instalar Entity Framework Core
### En Infraestructura (Quien hace el trabajo real con SQL)
dotnet add AutoFleet.Infrastructure/AutoFleet.Infrastructure.csproj package Microsoft.EntityFrameworkCore.SqlServer

### En API (Quien tiene las herramientas para ejecutar comandos)
dotnet add AutoFleet.API/AutoFleet.API.csproj package Microsoft.EntityFrameworkCore.Design

## 2.2 Gestión de Migraciones (SQL Server)
### Crear la migración inicial (Genera el código C# para crear tablas)
dotnet ef migrations add InitialCreate --project AutoFleet.Infrastructure --startup-project AutoFleet.API

### Aplicar cambios a la BD (Ejecuta el SQL)
dotnet ef database update --project AutoFleet.Infrastructure --startup-project AutoFleet.API

# 3. Arquitectura Explicada (Analogía de un Restaurante)
## 🏨 La Base de Datos (Docker / SQL Server)
Es el Local del Restaurante. Docker nos permite montar el local en cualquier máquina sin instalar cemento (software) permanente.

## 🤵 La API (VehiclesController.cs) - Capa Presentación
El Mesero.

Recibe al cliente (Postman/React).

Toma la orden (POST /api/vehicles).

Regla: No cocina. Solo pasa la nota. Si cambias al mesero por una App, la cocina sigue igual.

## 👨‍🍳 La Application (VehicleService.cs) - Capa Aplicación
El Chef.

Recibe la nota.

Valida reglas de negocio ("¿Hay ingredientes?", "No aceptamos autos del año 1800").

Pide ingredientes al almacén.

Prepara el plato final (DTO).

Nota: El DTO es el menú. No le das al cliente la vaca cruda (Entidad), le das la hamburguesa (DTO).

## ⚛️ El Core (Vehicle.cs) - Capa Dominio
Las Leyes de la Física.

Define QUÉ es un vehículo.

Contiene las Interfaces (Contratos): "Necesito alguien que sepa guardar datos".

Es el corazón puro, sin dependencias externas.

## 🏭 La Infrastructure (VehicleRepository.cs) - Capa Infraestructura
El Almacén.

Es el único que sabe que usamos SQL Server o Mongo.

Implementa el contrato del Core. Traduce "Guardar" a INSERT INTO....

# 4. Features Implementados
🧩 Feature: Persistencia Políglota (MongoDB)
## Agregamos soporte para bases de datos NoSQL.
dotnet add AutoFleet.Infrastructure/AutoFleet.Infrastructure.csproj package MongoDB.Driver

## 🧮 Feature: Optimizador de Flota (Algoritmo DP)
Problema: Transportar N pasajeros con el mínimo de vehículos.
Solución: Algoritmo Change Making Problem (tipo Mochila).

### Actualización de BD para soportar capacidades y consumo
dotnet ef migrations add AddCapacityAndStatus --project AutoFleet.Infrastructure --startup-project AutoFleet.API
dotnet ef database update --project AutoFleet.Infrastructure --startup-project AutoFleet.API

## 🔒 Feature: Autenticación y Seguridad (JWT + BCrypt)
Protección de la API con Tokens y Hashing de contraseñas.
### Paquetes en API (Para validar el token)
dotnet add AutoFleet.API/AutoFleet.API.csproj package Microsoft.AspNetCore.Authentication.JwtBearer

## Feature Swagger documentación

### Paquetes para using Microsoft.OpenApi.Models;
dotnet add AutoFleet.API/AutoFleet.API.csproj package Microsoft.OpenApi
dotnet add AutoFleet.API/AutoFleet.API.csproj package Swashbuckle.AspNetCore

### Paquetes en Application (Para generar token y hashear pass)
dotnet add AutoFleet.Application/AutoFleet.Application.csproj package System.IdentityModel.Tokens.Jwt
dotnet add AutoFleet.Application/AutoFleet.Application.csproj package Microsoft.Extensions.Configuration.Abstractions
dotnet add AutoFleet.Application/AutoFleet.Application.csproj package BCrypt.Net-Next

### Migración para tabla de Usuarios
dotnet ef migrations add AddUsersTable --project AutoFleet.Infrastructure --startup-project AutoFleet.API
dotnet ef database update --project AutoFleet.Infrastructure --startup-project AutoFleet.API

# 5. Pruebas y Calidad (Testing)
## 5.1 Configuración del Proyecto de Tests
### Crear proyecto xUnit
dotnet new xunit -n AutoFleet.Tests
dotnet sln add AutoFleet.Tests/AutoFleet.Tests.csproj

### Referencias (Testea Application usando Core)
dotnet add AutoFleet.Tests/AutoFleet.Tests.csproj reference AutoFleet.Application/AutoFleet.Application.csproj
dotnet add AutoFleet.Tests/AutoFleet.Tests.csproj reference AutoFleet.Core/AutoFleet.Core.csproj

### Instalar Moq (Para simular dependencias falsas)
dotnet add AutoFleet.Tests/AutoFleet.Tests.csproj package Moq

## 5.2 🧪 GUÍA DE EJECUCIÓN DE PRUEBAS (Desde Cero)
### A. Pruebas Unitarias (Automáticas)
Estas pruebas verifican la lógica matemática y de negocio sin tocar la base de datos real.

Comando: dotnet test
Qué valida: Que el algoritmo de optimización seleccione correctamente los vehículos y calcule bien el consumo de gasolina simulado.

### B. Pruebas Manuales / Integración (Swagger)
Escenario: Base de datos vacía. Queremos probar el sistema completo.

Levantar la API:
dotnet run --project AutoFleet.API

Ve a: https://localhost:7xxx/swagger

Paso 1: Crear Usuario (Registro) 
Endpoint: POST /api/auth/register

Body: { "username": "Admin", "password": "Password123!" }

Paso 2: Obtener Token (Login)

Endpoint: POST /api/auth/login

Body: (El mismo de arriba)

Acción: Copia el token de la respuesta. Ve al botón Authorize (candado) arriba a la derecha y escribe: Bearer TU_TOKEN_AQUI.

Paso 3: Sembrar Datos (Crear Flota)

Endpoint: POST /api/vehicles (Ejecutar 3 veces con estos datos):

Vehículo 1 (Eficiente): { "vin": "TSLA-26", "brand": "Tesla", "model": "Model Y", "year": 2026, "price": 55000, "passengerCapacity": 4, "kmPerLiter": 15 }

Vehículo 2 (Gastón): { "vin": "HUMM-10", "brand": "Hummer", "model": "H2", "year": 2010, "price": 40000, "passengerCapacity": 4, "kmPerLiter": 5 }

Vehículo 3 (Bus): { "vin": "BUS-01", "brand": "Mercedes", "model": "Sprinter", "year": 2024, "price": 90000, "passengerCapacity": 15, "kmPerLiter": 9 }

Paso 4: Probar la Inteligencia (Optimización)

Endpoint: POST /api/fleet/optimize

Body: { "totalPassengers": 4 }

Resultado Esperado: Debe elegir el Tesla (15 km/l) sobre la Hummer, demostrando que el algoritmo prioriza eficiencia.

# 6. Mantenimiento y Utilidades
Cliente de Consola (Simulador)
## Cliente externo para pruebas de carga o integración simple usando Polly para resiliencia.

dotnet new console -n AutoFleet.ConsoleClient
dotnet sln add AutoFleet.ConsoleClient/AutoFleet.ConsoleClient.csproj
dotnet add AutoFleet.ConsoleClient/AutoFleet.ConsoleClient.csproj package Microsoft.Extensions.Http.Polly
dotnet add AutoFleet.ConsoleClient/AutoFleet.ConsoleClient.csproj package Newtonsoft.Json

## Reinicio Nuclear (Borrar BD y empezar de cero)
 ⚠️ Peligro: Esto borra todos los datos.
### 1. Borrar la BD física
dotnet ef database drop --project AutoFleet.Infrastructure --startup-project AutoFleet.API --force

### 2. (Opcional) Borrar carpeta Migrations manualmente si se quiere limpiar el historial de código

### 3. Regenerar migración inicial
dotnet ef migrations add InitialCreate --project AutoFleet.Infrastructure --startup-project AutoFleet.API

### 4. Crear BD nueva
dotnet ef database update --project AutoFleet.Infrastructure --startup-project AutoFleet.API
