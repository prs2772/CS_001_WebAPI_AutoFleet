// Basic validations, Fail Fast concept. Filters trash before reaches Domain. Such as Format or length. 
using System.ComponentModel.DataAnnotations;

namespace AutoFleet.Application.DTOs;

/// <summary>
/// In-DTO Used to add a new vehicle to the Database
/// </summary>
public class CreateVehicleDto
{
    [Required(ErrorMessage = "VIN number is obligatory")]
    [StringLength(17, MinimumLength = 17, ErrorMessage = "VIN must be exactly 17 characters")]
    public string Vin { get; set; }

    [Required]
    public string Brand { get; set; }

    [Required]
    public string Model { get; set; }

    [Range(1900, 2100)]
    public int Year { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
    
    [Range(1, int.MaxValue)]
    public int PassengerCapacity { get; set; }
}
