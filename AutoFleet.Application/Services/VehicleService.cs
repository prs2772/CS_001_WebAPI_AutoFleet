using AutoFleet.Application.DTOs;
using AutoFleet.Application.Interfaces;
using AutoFleet.Core.Entities;
using AutoFleet.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace AutoFleet.Application.Services;

public class VehicleService : IVehicleService
{
    // Accessing the real database through the repository
    private readonly ILogger<VehicleService> _logger; // <--- 2. El campo privado
    private readonly IEnumerable<IVehicleRepository> _repositories;

    public VehicleService(IEnumerable<IVehicleRepository> repositories, ILogger<VehicleService> logger)
    {
        _repositories = repositories;
        _logger = logger;
    }

    // Implementing a failover pattern to read from primary but if it fails/doesnt find we look on the other sources.
    public async Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync()
    {
        // Iterating over all available repositories in order. IMPORTANT: The order of registration in DI matters, the first one that succeeds will be the one used.
        foreach (var repo in _repositories)
        {
            try
            {
                var vehicles = await repo.GetAllAsync();
                // Sucess criteria: Not null and retrieve data.
                // If we get an empty list it is valid, but if connection fails, goes to catch block and tries with next repository.

                // If in a future, empty list means try next, uncomment next code
                if (vehicles != null /* && vehicles.Any() */)
                {
                    return vehicles.Select(v => new VehicleDto
                    {
                        Brand = v.Brand,
                        Model = v.Model,
                        Price = v.Price,
                        KmPerLiter = v.KmPerLiter
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Failing when tried to obtain vehicles from source {repo.GetType().Name}. Trying next source...");
                continue;
            }
        }

        // If reaches this part of code, all repositories have failed
        _logger.LogError("All repositories failed when tried to get vehicles.");
        return [];
    }

    public async Task<VehicleDto> CreateVehicleAsync(CreateVehicleDto dto)
    {
        var vehicle = new Vehicle
        {
            Vin = dto.Vin,
            Brand = dto.Brand,
            Model = dto.Model,
            Year = dto.Year,
            Price = dto.Price,
            IsSold = false,
            PassengerCapacity = dto.PassengerCapacity,
            KmPerLiter = dto.KmPerLiter
        };

        // Iterating all data sources (SQL and Mongo) to persist the vehicle
        foreach (var repo in _repositories)
        {
            await repo.AddAsync(vehicle);
        }

        return new VehicleDto
        {
            Brand = vehicle.Brand,
            Model = vehicle.Model,
            Price = vehicle.Price
        };
    }
}
