# 1. IMAGEN BASE (El entorno donde correrá la app)
# Usamos ASP.NET Core Runtime (es ligero, solo para correr)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# 2. IMAGEN DE CONSTRUCCIÓN (El SDK completo para compilar)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos los csproj y restauramos dependencias (Capa por capa para caché)
COPY ["AutoFleet.API/AutoFleet.API.csproj", "AutoFleet.API/"]
COPY ["AutoFleet.Application/AutoFleet.Application.csproj", "AutoFleet.Application/"]
COPY ["AutoFleet.Core/AutoFleet.Core.csproj", "AutoFleet.Core/"]
COPY ["AutoFleet.Infrastructure/AutoFleet.Infrastructure.csproj", "AutoFleet.Infrastructure/"]

RUN dotnet restore "AutoFleet.API/AutoFleet.API.csproj"

# Copiamos todo el código fuente
COPY . .
WORKDIR "/src/AutoFleet.API"

# Compilamos
RUN dotnet build "AutoFleet.API.csproj" -c Release -o /app/build

# 3. PUBLICACIÓN
FROM build AS publish
RUN dotnet publish "AutoFleet.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. IMAGEN FINAL (Solo lo necesario para producción)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AutoFleet.API.dll"]
