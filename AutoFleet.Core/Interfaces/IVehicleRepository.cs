using AutoFleet.Core.Entities;
using AutoFleet.Core.Models;

namespace AutoFleet.Core.Interfaces
{
    public interface IVehicleRepository
    {
        #region SingleVehicle Management
        Task<IEnumerable<Vehicle>> GetAllAsync();
        Task<Vehicle?> GetByIdAsync(int id);
        Task AddAsync(Vehicle vehicle);

        #endregion

        #region Fleet Management
        // 20260221 + PRS: Added contact to get summary of available fleet
        Task<List<InventoryItem>> GetAvailableFleetSummaryAsync();

        #endregion
    }
}
