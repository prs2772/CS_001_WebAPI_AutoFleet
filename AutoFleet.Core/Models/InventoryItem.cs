namespace AutoFleet.Core.Models;

public class InventoryItem
{
    public string VehicleName { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int AvailableCount { get; set; }
    public decimal KmPerLiter { get; set; }
    public int Year { get; set; }
}
