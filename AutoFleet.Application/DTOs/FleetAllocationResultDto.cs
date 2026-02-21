namespace AutoFleet.Application.DTOs;

public class FleetAllocationResultDto
{
    public bool IsPossible { get; set; }
    public int TotalVehiclesNeeded { get; set; } // Ejemplo: "Autobús" -> 2, "Van" -> 1
    public Dictionary<string, int> VehicleBreakdown { get; set; } = new();
    public List<string> DetailedList { get; set; } = new();
}
