using AutoFleet.Application.DTOs;

namespace AutoFleet.Application.Interfaces
{
    public interface IFleetOptimizerService
    {
        FleetAllocationResultDto OptimizeAllocation(int passengers);
    }
}
