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

        [HttpPost("optimize")]
        public async Task<IActionResult> GetGreedyFleet([FromBody] FleetRequestDto request)
        {
            // Sample of optimization complex algorithm validation
            if (request.TotalPassengers <= 0)
                return BadRequest("At least 1 passenger required.");

            var result = await _optimizer.OptimizeAllocationAsync(request.TotalPassengers);

            if (!result.IsPossible)
                return BadRequest($"No exact combination exists for {request.TotalPassengers} passenger(s)");

            return Ok(result);
        }
    }
}
