using Microsoft.AspNetCore.Mvc;
using AutoFleet.Application.Interfaces;
using AutoFleet.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

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


        /* ----------------------------------------------------------------------------------
        INTEGRACIÓN API EXTERNA (NHTSA)
        ----------------------------------------------------------------------------------
        Objetivo:    Consumir servicio REST de terceros (Requisito de la guía).
        Fuente:      NHTSA vPIC API (Gobierno USA - Open Data).
        Doc Oficial: https://vpic.nhtsa.dot.gov/api/
        
        Proceso de Integración:
        1. Endpoint seleccionado: GetModelsForMake
        2. Truco técnico: Esta API retorna XML por defecto. Se debe anexar '?format=json'
            en el QueryString para interoperabilidad moderna.
        3. Arquitectura: Implementado como un "Proxy Passthrough" en el controlador
            para demostración rápida. En producción, esto iría en un servicio 
            'IVehicleInfoProvider' dentro de Infraestructura.
        ----------------------------------------------------------------------------------
        */
        /// <summary>
        /// [PROXY] Consulta la base de datos gubernamental de la NHTSA (USA) para obtener modelos.
        /// </summary>
        /// <remarks>
        /// Este endpoint actúa como un Gateway hacia la API pública vPIC (Vehicle Product Information Catalog).
        /// Útil para validar marcas o poblar selectores de modelos sin mantener una base de datos propia.
        /// <br/>
        /// <strong>Fuente:</strong> https://vpic.nhtsa.dot.gov/api/
        /// </remarks>
        /// <param name="make">Nombre de la marca (ej. "tesla", "ford", "bmw").</param>
        /// <returns>JSON crudo con la lista de modelos oficiales.</returns>
        /// <response code="200">Retorna la lista de modelos en formato JSON.</response>
        /// <response code="502">Bad Gateway. Error de conexión con la API externa.</response>
        [HttpGet("external-models/{make}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        public async Task<IActionResult> GetModelsFromExternalApi(string make)
        {
            // 1. Instanciamos HttpClient (En PROD usar IHttpClientFactory)
            using var httpClient = new HttpClient();
            
            // 2. Construimos la URL
            // Tip de investigación: La API por defecto devuelve XML, forzamos JSON con ?format=json
            var url = $"https://vpic.nhtsa.dot.gov/api/vehicles/getmodelsformake/{make}?format=json";

            try 
            {
                // 3. Llamada Asíncrona
                var response = await httpClient.GetAsync(url);
                
                if (!response.IsSuccessStatusCode)
                    return StatusCode(StatusCodes.Status502BadGateway, "Error al conectar con NHTSA");

                var jsonString = await response.Content.ReadAsStringAsync();
                
                // Retornamos el contenido tal cual (Proxy Passthrough)
                return Content(jsonString, "application/json");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error interno: {ex.Message}");
            }
        }
    }
}
