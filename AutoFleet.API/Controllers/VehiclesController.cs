using Microsoft.AspNetCore.Mvc;
using AutoFleet.Application.Interfaces;
using AutoFleet.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace AutoFleet.API.Controllers
{
    [Authorize] // Authentication required
    [ApiController] // Automatic validations and API behavior
    [Route("api/[controller]")] // Url: MyIP... api/vehicles
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        // Dependency Injection: Asking SERVICE as Interface, not the concretion? (O concreción)
        public VehiclesController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        /// <summary>
        /// Obtiene todos los vehículos registrados en la base de datos.
        /// </summary>
        /// <returns>Lista completa de vehículos.</returns>
        /// <response code="200">Devuelve la lista de vehículos.</response>
        /// <response code="401">No autenticado.</response>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var vehicles = await _vehicleService.GetAllVehiclesAsync();
            return Ok(vehicles); // List of vehicles
        }

        /// <summary>
        /// Crea un nuevo vehículo en el inventario.
        /// </summary>
        /// <remarks>
        /// Valida los datos de entrada según las reglas de negocio (año, rango de precios, etc.).
        /// </remarks>
        /// <param name="vehicleDto">Datos del vehículo a crear.</param>
        /// <returns>El vehículo creado con su ID asignado.</returns>
        /// <response code="201">Vehículo creado exitosamente.</response>
        /// <response code="400">Datos de entrada inválidos.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateVehicleDto vehicleDto)
        {
            // [ApiController] Validates the DTO (Required, StringLength, etc.)
            // If it is not valid data, we return BadRequest (400) auto

            try 
            {
                var createdVehicle = await _vehicleService.CreateVehicleAsync(vehicleDto);
                
                // Retorna 201 Created
                return CreatedAtAction(nameof(GetAll), new { }, createdVehicle);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno: " + ex.Message);
            }
        }
    }
}
