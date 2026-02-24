using Microsoft.AspNetCore.Mvc;
using AutoFleet.Application.Interfaces;
using AutoFleet.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace AutoFleet.API.Controllers;

[Authorize] // Authentication required
[ApiController]
[Route("api/[controller]")]
public class FleetController : ControllerBase
{
    private readonly IFleetOptimizerService _optimizer;

    public FleetController(IFleetOptimizerService optimizer)
    {
        _optimizer = optimizer;
    }

    /// <summary>
    /// Calcula una asignación de flota simple (Algoritmo Voraz básico).
    /// </summary>
    /// <remarks>
    /// Intenta llenar la capacidad usando los vehículos más grandes primero, sin considerar
    /// exhaustivamente la eficiencia de combustible o combinaciones complejas.
    /// </remarks>
    /// <param name="request">Número de pasajeros.</param>
    /// <returns>Resultado de la asignación.</returns>
    /// <response code="200">Asignación calculada correctamente.</response>
    /// <response code="400">Si el número de pasajeros es inválido o no hay vehículos suficientes.</response>
    /// <response code="401">No autenticado.</response>
    [HttpPost("simple")]
    public async Task<IActionResult> GetSimpleFleet([FromBody] FleetRequestDto request)
    {
        // Sample of simple validation
        if (request.TotalPassengers <= 0)
            return BadRequest("At least 1 passenger required.");

        var result = await _optimizer.GetSimpleAllocationAsync(request.TotalPassengers);

        if (!result.IsPossible)
            return BadRequest($"No available vehicles exists for covering {request.TotalPassengers} passenger(s)");

        return Ok(result);
    }

    /// <summary>
    /// Calcula la combinación óptima de vehículos para un número de pasajeros.
    /// </summary>
    /// <remarks>
    /// Utiliza un algoritmo de Programación Dinámica para minimizar el número de vehículos
    /// priorizando la eficiencia de combustible y en segundo lugar la cantidad de vehículos (choferes).
    /// </remarks>
    /// <param name="request">DTO con el número total de pasajeros.</param>
    /// <returns>Detalle de la flota asignada y consumo promedio.</returns>
    /// <response code="200">Retorna la asignación exitosa.</response>
    /// <response code="400">Si no hay suficientes vehículos o la combinación es imposible.</response>
    /// <response code="401">Si el usuario no está autenticado.</response>
    [HttpPost("optimize")]
    public async Task<IActionResult> GetOptimalFleet([FromBody] FleetRequestDto request)
    {
        // Sample of optimization complex algorithm validation
        if (request.TotalPassengers <= 0)
            return BadRequest("At least 1 passenger required.");

        var result = await _optimizer.OptimizeAllocationAsync(request.TotalPassengers);

        if (!result.IsPossible)
            return BadRequest($"No exact vehicles exists for {request.TotalPassengers} passenger(s)");

        return Ok(result);
    }
}
