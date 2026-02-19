using AutoFleet.Application.DTOs;
using AutoFleet.Application.Interfaces;
using AutoFleet.Core.Entities;
using AutoFleet.Core.Interfaces;

namespace AutoFleet.Application.Services
{
    public class VehicleService : IVehicleService
    {
        // CAMBIO: Ahora recibimos una LISTA de repositorios (IEnumerable)
        private readonly IEnumerable<IVehicleRepository> _repositories;

        public VehicleService(IEnumerable<IVehicleRepository> repositories)
        {
            _repositories = repositories;
        }

        public async Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync()
        {
            // Para leer, usualmente elegimos UNA fuente principal.
            // Aquí tomamos el primero (que será SQL Server por orden de registro)
            var primaryRepo = _repositories.FirstOrDefault();

            if (primaryRepo != null)
            {
                var vehicles = await primaryRepo.GetAllAsync();
                return vehicles.Select(v => new VehicleDto
                {
                    Brand = v.Brand,
                    Model = v.Model,
                    Price = v.Price
                });
            }
            return new List<VehicleDto>();
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
                IsSold = false
            };

            // LA MAGIA: Iteramos sobre todos los repositorios registrados
            // y guardamos en todos (SQL y Mongo)
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
}
