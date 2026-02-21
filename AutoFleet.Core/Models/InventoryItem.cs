namespace AutoFleet.Core.Models;

public class InventoryItem
{
    public string VehicleName { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int AvailableCount { get; set; }
}
