namespace AutoFleet.Core.Entities;

// Are defined states and fixed to the business rules. They are not changing. Thats why I put them as an enum
public enum VehicleStatus
{
    Available = 1,// Ready to Use
    Rented = 2,// On use
    Maintenance = 3 // Not available due to repairs or upkeep
}
