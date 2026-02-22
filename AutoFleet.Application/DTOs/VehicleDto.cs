namespace AutoFleet.Application.DTOs;

/// <summary>
/// OUT-DTO Vehicle information shown to the user as reference
/// </summary>
public class VehicleDto
{
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal KmPerLiter { get; set; }
}
