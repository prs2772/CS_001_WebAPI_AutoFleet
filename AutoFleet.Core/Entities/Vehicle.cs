using System.ComponentModel.DataAnnotations;

namespace AutoFleet.Core.Entities;

public class Vehicle
{
    public int Id { get; set; }
    public string Vin { get; set; } = string.Empty; // Vehicle Identification Number
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    [Range(1900, 2059)] // Business rule validation: Year must be reasonable, avoids mistyping errors.
    public int Year { get; set; }
    public decimal Price { get; set; }
    public bool IsSold { get; set; }

    // 20260221 + PRS: Added new fields to comply with requirements
    public int PassengerCapacity { get; set; }
    public VehicleStatus Status { get; set; } = VehicleStatus.Available;
}
