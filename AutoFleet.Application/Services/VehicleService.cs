using AutoFleet.Application.DTOs;
using AutoFleet.Application.Interfaces;
using AutoFleet.Core.Entities;
using AutoFleet.Core.Interfaces;

namespace AutoFleet.Application.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _repository;

        public VehicleService(IVehicleRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync()
        {
            var vehicles = await _repository.GetAllAsync();
            
            // Mapeo manual (luego usaremos AutoMapper si quieres)
            return vehicles.Select(v => new VehicleDto 
            { 
                Brand = v.Brand, 
                Model = v.Model, 
                Price = v.Price 
            });
        }

        public async Task<VehicleDto> CreateVehicleAsync(CreateVehicleDto dto)
        {
            // Aquí podrías validar reglas de negocio complejas (ej: "No aceptar autos anteriores a 1950")
            
            var vehicle = new Vehicle
            {
                Vin = dto.Vin,
                Brand = dto.Brand,
                Model = dto.Model,
                Year = dto.Year,
                Price = dto.Price,
                IsSold = false
            };

            await _repository.AddAsync(vehicle);

            return new VehicleDto 
            { 
                Brand = vehicle.Brand, 
                Model = vehicle.Model, 
                Price = vehicle.Price 
            };
        }
    }
}
