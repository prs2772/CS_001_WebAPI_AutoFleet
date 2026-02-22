using Microsoft.EntityFrameworkCore;
using AutoFleet.Core.Entities;
using AutoFleet.Core.Interfaces;
using AutoFleet.Infrastructure.Data;
using AutoFleet.Application.DTOs;
using AutoFleet.Core.Models;
using AutoFleet.Core.Enums;

namespace AutoFleet.Infrastructure.Repositories;

public class VehicleRepository : IVehicleRepository
{
    public RepositorySource Source { get; } = RepositorySource.MICROSOFT_SQL;

    private readonly AutoFleetDbContext _context;

    public VehicleRepository(AutoFleetDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Vehicle>> GetAllAsync()
    {
        return await _context.Vehicles.ToListAsync();
    }

    public async Task<Vehicle?> GetByVinAsync(string vin)
    {
        return await _context.Vehicles.FirstOrDefaultAsync(v => v.Vin == vin);
    }

    public async Task AddAsync(Vehicle vehicle)
    {
        await _context.Vehicles.AddAsync(vehicle);
        await _context.SaveChangesAsync();
    }

    // 20260221 + PRS: Added method to get summary of available fleet
    public async Task<List<InventoryItem>> GetAvailableFleetSummaryAsync()
    {
        // SQL: SELECT Brand + ' ' + Model, PassengerCapacity, COUNT(*) FROM Vehicles WHERE Status = 1 GROUP BY Brand, Model, PassengerCapacity
        var inventory = await _context.Vehicles
            .Where(v => v.Status == VehicleStatus.Available)
            .GroupBy(v => new { v.Brand, v.Model, v.PassengerCapacity, v.KmPerLiter, v.Year })
            .Select(g => new InventoryItem
            {
                VehicleName = g.Key.Brand + " " + g.Key.Model,
                Capacity = g.Key.PassengerCapacity,
                KmPerLiter = g.Key.KmPerLiter,
                Year = g.Key.Year,
                AvailableCount = g.Count(),
            })
            .ToListAsync();

        return inventory;
    }
}
