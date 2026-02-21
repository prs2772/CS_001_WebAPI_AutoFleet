namespace AutoFleet.Core.Entities;

public class Vehicle
{
    public int Id { get; set; }
    public string Vin { get; set; } = string.Empty; // Vehicle Identification Number
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public decimal Price { get; set; }
    public bool IsSold { get; set; }

    // 20260221 + PRS: Added new fields to comply with requirements
    public int PassengerCapacity { get; set; }
    public VehicleStatus Status { get; set; } = VehicleStatus.Available;
}
