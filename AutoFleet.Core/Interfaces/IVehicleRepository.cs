using AutoFleet.Core.Entities;
using AutoFleet.Core.Models;
using AutoFleet.Core.Enums;

namespace AutoFleet.Core.Interfaces;

/// <summary>
/// Manages our contract for Vehicle operations, both for single vehicles and fleet summaries.
/// </summary>
public interface IVehicleRepository
{
    /// <summary>
    /// Identifies the source as MSSQL or Mongo
    /// </summary>
    RepositorySource Source { get; }

    #region SingleVehicle Management
    Task<IEnumerable<Vehicle>> GetAllAsync();
    Task<Vehicle?> GetByIdAsync(int id);
    Task AddAsync(Vehicle vehicle);

    #endregion

    #region Fleet Management
    // 20260221 + PRS: Added contact to get summary of available fleet
    /// <summary>
    /// Obtains a list of each Vehicle grouped by Name and their availability at the moment
    /// </summary>
    /// <returns>List of different vehicles availables</returns>
    Task<List<InventoryItem>> GetAvailableFleetSummaryAsync();

    #endregion
}
