using Microsoft.AspNetCore.Mvc;
using AutoFleet.Application.Interfaces;
using AutoFleet.Application.DTOs;

namespace AutoFleet.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FleetController : ControllerBase
    {
        private readonly IFleetOptimizerService _optimizer;

        public FleetController(IFleetOptimizerService optimizer)
        {
            _optimizer = optimizer;
        }

        [HttpPost("optimize")]
        public async Task<IActionResult> Optimize([FromBody] FleetRequestDto request)
        {
            // Validación simple
            if (request.TotalPassengers <= 0)
                return BadRequest("Se requiere al menos 1 pasajero.");

            // Validar que sea posible llenar exactamente con múltiplos de 4 (si solo tenemos Sedan de 4)
            // Ojo: Mi algoritmo asume cambio exacto. Si pides 3 pasajeros y el mínimo es 4, fallará.
            // En PROD: Redondeamos hacia arriba al múltiplo más cercano.
            
            // PEQUEÑO TRUCO DE NEGOCIO:
            // Si el número no es par o no encaja, podríamos "rellenar" artificialmente,
            // pero probemos el algoritmo puro primero.
            
            var result = await _optimizer.OptimizeAllocationAsync(request.TotalPassengers);

            if (!result.IsPossible)
                return BadRequest("No tenemos combinación exacta de vehículos para ese número de pasajeros (Intenta múltiplos de 2 o 4).");

            return Ok(result);
        }
    }
}
