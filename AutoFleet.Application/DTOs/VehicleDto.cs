namespace AutoFleet.Application.DTOs;

/// <summary>
/// OUT-DTO Vehicle information shown to the user as reference
/// </summary>
public class VehicleDto
{
    public string Brand { get; set; }
    public string Model { get; set; }
    public decimal Price { get; set; }
}
