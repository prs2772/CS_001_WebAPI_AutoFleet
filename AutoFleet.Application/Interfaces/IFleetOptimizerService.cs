using AutoFleet.Application.DTOs;

namespace AutoFleet.Application.Interfaces
{
    public interface IFleetOptimizerService
    {
        Task<FleetAllocationResultDto> OptimizeAllocationAsync(int passengers);
    }
}
