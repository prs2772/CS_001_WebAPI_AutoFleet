using AutoFleet.Application.DTOs;
using AutoFleet.Application.Interfaces;
using AutoFleet.Core.Entities;
using AutoFleet.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using AutoFleet.Core.Enums;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace AutoFleet.Application.Services;

public class AuthService : IAuthService
{
    // Inyectamos la Interfaz, no el DbContext
    private readonly IUserRepository _userRepository; 
    private readonly IConfiguration _config;

    public AuthService(IUserRepository userRepository, IConfiguration config)
    {
        _userRepository = userRepository;
        _config = config;
    }

    public async Task<User?> RegisterAsync(RegisterUserDto dto)
    {
        // Usamos la interfaz
        if (await _userRepository.UserExistsAsync(dto.Username))
            return null;

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = passwordHash,
            Role = UserRole.USER
        };

        // Usamos la interfaz
        await _userRepository.AddAsync(user);
            
        return user;
    }

    public async Task<string?> LoginAsync(LoginDto dto)
    {
        // Usamos la interfaz
        var user = await _userRepository.GetByUsernameAsync(dto.Username);
            
        if (user == null) return null;

        bool isValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            
        if (!isValid) return null;

        return GenerateJwtToken(user);
    }

    private string GenerateJwtToken(User user)
    {
        // (El código del token se mantiene igual, ya que System.IdentityModel es una librería estándar de .NET, no infraestructura pura)
        var keyStr = _config["Jwt:Key"];
        if (string.IsNullOrEmpty(keyStr))
        {
            throw new InvalidOperationException("Jwt key not found! API Cannot be initiated");
        }
        var key = Encoding.ASCII.GetBytes(keyStr);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
