using AutoFleet.Application.DTOs;

namespace AutoFleet.Application.Interfaces;

/// <summary>
/// Manages vehicle-related operations, such as retrieving available vehicles and adding new ones.
/// </summary>
public interface IVehicleService
{
    Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync();
    Task<VehicleDto> CreateVehicleAsync(CreateVehicleDto vehicleDto);
}

// The reasons to add interfaces, are multiple besides Dependency Injection, but the main ones are:
// 1. Unit Testing: Moq, allows moquing interfaces, but not concrete classes. With interfaces, you can create mock implementations for testing without relying on the actual database or external services.
// 2. Decorators / Proxies: If tomorrow you want to add Caching or Logging to all your methods without touching the service code, we can use the Decorator pattern over the interface.
// 3. Public Contract: The interface tells the world (the API) what you do, hiding how you do it.
