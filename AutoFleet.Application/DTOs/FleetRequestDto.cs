using System.ComponentModel.DataAnnotations;

namespace AutoFleet.Application.DTOs;

/// <summary>
/// IN-DTO Amount of passengers to allocate
/// </summary>
public class FleetRequestDto
{
    [Range(1, int.MaxValue)]
    public int TotalPassengers { get; set; }
}
