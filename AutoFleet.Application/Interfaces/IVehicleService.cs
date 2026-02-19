using AutoFleet.Application.DTOs;

namespace AutoFleet.Application.Interfaces
{
    public interface IVehicleService
    {
        Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync();
        Task<VehicleDto> CreateVehicleAsync(CreateVehicleDto vehicleDto);
    }
}
