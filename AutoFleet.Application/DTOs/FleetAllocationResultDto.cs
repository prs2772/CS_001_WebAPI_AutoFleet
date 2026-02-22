namespace AutoFleet.Application.DTOs;

/// <summary>
/// OUT-DTO Tells the user whether an allocation of passengers is possible, how many vehicles are needed and the list
/// </summary>
public class FleetAllocationResultDto
{
    public bool IsPossible { get; set; }
    public int TotalVehiclesNeeded { get; set; }
    public Dictionary<string, int> VehicleBreakdown { get; set; } = new();
    public List<string> DetailedList { get; set; } = new();
}
