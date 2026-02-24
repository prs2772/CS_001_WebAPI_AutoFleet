using Microsoft.AspNetCore.Mvc;
using AutoFleet.Application.DTOs;
using AutoFleet.Application.Interfaces;

namespace AutoFleet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto request)
    {
        var user = await _authService.RegisterAsync(request);
        if (user == null)
            return BadRequest("El usuario ya existe.");

        return Ok(new { message = "Usuario registrado exitosamente" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        var token = await _authService.LoginAsync(request);

        if (token == null)
            return Unauthorized("Usuario o contraseña incorrectos."); // Security Message

        return Ok(new { Token = token });
    }
}
