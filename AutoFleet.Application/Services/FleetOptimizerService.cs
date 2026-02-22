using AutoFleet.Application.DTOs;
using AutoFleet.Application.Interfaces;
using AutoFleet.Core.Interfaces;
using AutoFleet.Core.Enums;

namespace AutoFleet.Application.Services;

public class FleetOptimizerService : IFleetOptimizerService
{
    // Accessing the real database through the repository
    private readonly IVehicleRepository _repository;

    public FleetOptimizerService(IEnumerable<IVehicleRepository> repositories)
    {
        _repository = repositories.FirstOrDefault(r => r.Source == RepositorySource.MICROSOFT_SQL) 
                      ?? repositories.First();
    }

    public async Task<FleetAllocationResultDto> GetSimpleAllocationAsync(int targetPassengers)
    {
        // 1. Fetching the data from repository
        var availableFleet = await _repository.GetAvailableFleetSummaryAsync();

        // 2. Sorting by DESC with greedy strategy
        var sortedFleet = availableFleet.OrderByDescending(x => x.Capacity).ToList();

        var result = new FleetAllocationResultDto();
        int remainingPassengers = targetPassengers;

        // 3. Assignment algorithm with finite stock which is the fleet summary
        foreach (var vehicleType in sortedFleet)
        {
            if (remainingPassengers <= 0) break;

            // How many of this type do I need if I had infinite stock?
            int needed = remainingPassengers / vehicleType.Capacity;

            // How many do I have available? (Real stock)
            int take = Math.Min(needed, vehicleType.AvailableCount);

            // I have vehicles to take, I take all I can of this type
            if (take > 0)
            {
                result.VehicleBreakdown.Add(vehicleType.VehicleName, take);

                for (int i = 0; i < take; i++) result.DetailedList.Add(vehicleType.VehicleName);

                remainingPassengers -= take * vehicleType.Capacity;
                result.TotalVehiclesNeeded += take;
            }
        }

        // 4. Remaining passengers handling.
        if (remainingPassengers > 0)
        {
            // Looking for a single vehicle that can take all remaining passengers if any, even if it is not the optimal one
            var smallBus = sortedFleet
                .Where(v => v.Capacity >= remainingPassengers)
                .OrderBy(v => v.Capacity) // The smallest possible to avoid overkill
                .FirstOrDefault(v => !result.VehicleBreakdown.ContainsKey(v.VehicleName) ||
                                     result.VehicleBreakdown[v.VehicleName] < v.AvailableCount);
            if (smallBus != null)
            {
                // if was found, we close the deal
                if (!result.VehicleBreakdown.ContainsKey(smallBus.VehicleName))
                    result.VehicleBreakdown[smallBus.VehicleName] = 0;

                result.VehicleBreakdown[smallBus.VehicleName]++;
                result.DetailedList.Add(smallBus.VehicleName);
                result.TotalVehiclesNeeded++;
                remainingPassengers = 0;
            }
        }

        result.IsPossible = remainingPassengers <= 0;

        if (!result.IsPossible)
        {
            // There were note enough vehicles to allocate all passengers
            result.DetailedList.Clear(); // Partial allocations are not useful for the user, we clear the list to avoid confusion
        }

        return result;
    }

    public async Task<FleetAllocationResultDto> OptimizeAllocationAsync(int targetPassengers)
    {
        // 1. Obtaining data and availability
        var inventory = await _repository.GetAvailableFleetSummaryAsync();

        // 2. Flattening ([bus(20), bus(20), car(4)] instead of bus(20) x2, car(4) x1)
        var allVehicles = new List<(string Name, int Capacity)>();
        foreach (var item in inventory)
        {
            for (int i = 0; i < item.AvailableCount; i++)
            {
                allVehicles.Add((item.VehicleName, item.Capacity));
            }
        }

        // 3. Dynamic Programming (DP) initialization
        // dp[i] = minimal mount of vehicles to reach i amount
        // parent[i] = vehicle index on 'allVehicles' qused to reach 'i'
        // prevStat[i] = Capacity prior to add the new vehicle (to rebuild the way)

        int maxCapacity = targetPassengers + 1; 
        int[] dp = new int[targetPassengers + 1];
        int[] parentVehicleIndex = new int[targetPassengers + 1];
        int[] previousState = new int[targetPassengers + 1]; // All values remain 0

        Array.Fill(dp, maxCapacity);// Filling all as capacity + 1 (infinite). When < than that value, menas solution was found
        Array.Fill(parentVehicleIndex, -1); // Filling all as -1
        dp[0] = 0; // For 0 passengers, minimum vehicles needed is 0

        // 4. DP Core
        for (int vIndex = 0; vIndex < allVehicles.Count; vIndex++)
        {
            var vehicle = allVehicles[vIndex];
            int cap = vehicle.Capacity;

            // Iterating backwards avoiding same vehicle usage (target -> cap). Problem 0/1 Knapsack
            for (int p = targetPassengers; p >= cap; p--)
            {
                // If I can touch (not infinite which on our case is represented by targetPassengers + 1)
                if (dp[p - cap] != maxCapacity)
                {
                    int newCount = dp[p - cap] + 1;

                    // Optimization: Less vehicles amount
                    if (newCount < dp[p])
                    {
                        dp[p] = newCount;
                        parentVehicleIndex[p] = vIndex; // Saving the used vehicle (position i on allVehicles)
                        previousState[p] = p - cap;     // Saved the way that has been followed (place)
                    }
                }
            }
        }

        // 5. Building the response
        var result = new FleetAllocationResultDto();

        // Was the result possible?
        if (dp[targetPassengers] == maxCapacity)
        {
            result.IsPossible = false;
            return result;
        }

        result.IsPossible = true;
        result.TotalVehiclesNeeded = dp[targetPassengers];

        // 6. If it was possible, Backtracking
        int currentP = targetPassengers; // Starting from the goal
        while (currentP > 0)
        {
            int vIdx = parentVehicleIndex[currentP];
            var vehicle = allVehicles[vIdx];

            result.DetailedList.Add(vehicle.Name); // The final vehicle that arrived

            // Adding brief
            if (!result.VehicleBreakdown.ContainsKey(vehicle.Name))
                result.VehicleBreakdown[vehicle.Name] = 0;

            result.VehicleBreakdown[vehicle.Name]++;

            // Going backwards n positions of capacity
            currentP = previousState[currentP];
        }

        return result;
    }
}
