using Microsoft.EntityFrameworkCore;
using AutoFleet.Core.Entities;
using AutoFleet.Core.Interfaces;
using AutoFleet.Infrastructure.Data;

namespace AutoFleet.Infrastructure.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly AutoFleetDbContext _context;

        public VehicleRepository(AutoFleetDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Vehicle>> GetAllAsync()
        {
            return await _context.Vehicles.ToListAsync();
        }

        public async Task<Vehicle?> GetByIdAsync(int id)
        {
            return await _context.Vehicles.FindAsync(id);
        }

        public async Task AddAsync(Vehicle vehicle)
        {
            await _context.Vehicles.AddAsync(vehicle);
            await _context.SaveChangesAsync();
        }
    }
}
