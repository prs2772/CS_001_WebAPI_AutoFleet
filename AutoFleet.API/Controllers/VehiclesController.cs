using Microsoft.AspNetCore.Mvc;
using AutoFleet.Application.Interfaces;
using AutoFleet.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace AutoFleet.API.Controllers
{
    [Authorize] // Requiere autenticación para acceder a cualquier endpoint
    [ApiController] // Habilita validaciones automáticas y comportamientos de API
    [Route("api/[controller]")] // La ruta será: api/vehicles
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        // Inyección de Dependencias: Pedimos el SERVICIO, no el Repositorio
        public VehiclesController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        // GET: api/vehicles
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var vehicles = await _vehicleService.GetAllVehiclesAsync();
            return Ok(vehicles); // Retorna HTTP 200 con la lista
        }

        // POST: api/vehicles
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateVehicleDto vehicleDto)
        {
            // El [ApiController] valida automáticamente el DTO (Required, StringLength, etc.)
            // Si no cumple, retorna BadRequest (400) automáticamente.

            try 
            {
                var createdVehicle = await _vehicleService.CreateVehicleAsync(vehicleDto);
                
                // Retorna 201 Created
                return CreatedAtAction(nameof(GetAll), new { }, createdVehicle);
            }
            catch (Exception ex)
            {
                // Manejo básico de errores (luego veremos el global)
                return StatusCode(500, "Error interno: " + ex.Message);
            }
        }
    }
}
