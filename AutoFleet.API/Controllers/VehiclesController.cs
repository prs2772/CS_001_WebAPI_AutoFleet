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

        // GET: api/vehicles
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var vehicles = await _vehicleService.GetAllVehiclesAsync();
            return Ok(vehicles); // List of vehicles
        }

        // POST: api/vehicles
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
