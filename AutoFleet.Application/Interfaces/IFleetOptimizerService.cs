using AutoFleet.Application.DTOs;

namespace AutoFleet.Application.Interfaces;

/// <summary>
/// Service responsible for getting if it is possible an allocation and gives the details
/// </summary>
public interface IFleetOptimizerService
{
    /// <summary>
    /// Given a number of passengers, calculates if it is possible to allocate them with the available vehicles and gives the details of the allocation. Takes greatest vehicles first to minimize the total number of vehicles needed.
    /// </summary>
    /// <param name="passengers">Number of passengers to allocate</param>
    /// <returns>A DTO with the allocation result</returns>
    Task<FleetAllocationResultDto> GetSimpleAllocationAsync(int passengers);

    /// <summary>
    /// Given a number of passengers, calculates if it is possible to allocate them with the available vehicles and gives the details of the allocation. Takes the necessary and exact amount 
    /// </summary>
    /// <param name="passengers">Number of passengers to allocate</param>
    /// <returns>A DTO with the exact allocation result taking most optimal result</returns>
    Task<FleetAllocationResultDto> OptimizeAllocationAsync(int passengers);
}
