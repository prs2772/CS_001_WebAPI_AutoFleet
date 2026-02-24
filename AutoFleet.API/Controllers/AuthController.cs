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

    /// <summary>
    /// Registra un nuevo usuario en el sistema.
    /// </summary>
    /// <remarks>
    /// Crea un usuario con rol por defecto. La contraseña será encriptada antes de guardarse.
    /// </remarks>
    /// <param name="request">Credenciales del nuevo usuario.</param>
    /// <returns>Mensaje de confirmación.</returns>
    /// <response code="200">Usuario creado exitosamente.</response>
    /// <response code="400">Si el nombre de usuario ya existe.</response>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto request)
    {
        var user = await _authService.RegisterAsync(request);
        if (user == null)
            return BadRequest("El usuario ya existe.");

        return Ok(new { message = "Usuario registrado exitosamente" });
    }

    /// <summary>
    /// Inicia sesión y obtiene un Token JWT.
    /// </summary>
    /// <remarks>
    /// Envía las credenciales para recibir un token Bearer que debe usarse en los headers
    /// de las peticiones protegidas (Authorization: Bearer {token}).
    /// </remarks>
    /// <param name="request">Credenciales de acceso.</param>
    /// <returns>Objeto JSON con el Token.</returns>
    /// <response code="200">Retorna el token de acceso.</response>
    /// <response code="401">Si el usuario o contraseña son incorrectos.</response>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        var token = await _authService.LoginAsync(request);

        if (token == null)
            return Unauthorized("Usuario o contraseña incorrectos."); // Security Message

        return Ok(new { Token = token });
    }
}
