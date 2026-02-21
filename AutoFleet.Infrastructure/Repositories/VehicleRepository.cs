using Microsoft.EntityFrameworkCore;
using AutoFleet.Core.Entities;
using AutoFleet.Core.Interfaces;
using AutoFleet.Infrastructure.Data;
using AutoFleet.Application.DTOs;

namespace AutoFleet.Infrastructure.Repositories;

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

    // 20260221 + PRS: Added method to get summary of available fleet
    public async Task<List<InventoryItemDto>> GetAvailableFleetSummaryAsync()
    {
        // SQL: SELECT Brand + ' ' + Model, PassengerCapacity, COUNT(*) FROM Vehicles WHERE Status = 1 GROUP BY Brand, Model, PassengerCapacity
        var inventory = await _context.Vehicles
            .Where(v => v.Status == VehicleStatus.Available)
            .GroupBy(v => new { v.Brand, v.Model, v.PassengerCapacity })
            .Select(g => new InventoryItemDto
            {
                VehicleName = g.Key.Brand + " " + g.Key.Model,
                Capacity = g.Key.PassengerCapacity,
                AvailableCount = g.Count()
            })
            .ToListAsync();

        return inventory;
    }
}
