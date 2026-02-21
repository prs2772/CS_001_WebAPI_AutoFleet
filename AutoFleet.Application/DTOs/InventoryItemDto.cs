namespace AutoFleet.Application.DTOs;

// Will tell how many vehicles there are of each type, their capacity, and how many are available, 
// User does not need to know the status of each individual vehicle, just how many are available for allocation
public class InventoryItemDto
{
    public string VehicleName { get; set; }
    public int Capacity { get; set; }
    public int AvailableCount { get; set; }
}
