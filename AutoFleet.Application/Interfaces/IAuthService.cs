using AutoFleet.Application.DTOs;
using AutoFleet.Core.Entities;

namespace AutoFleet.Application.Interfaces;

public interface IAuthService
{
    Task<User?> RegisterAsync(RegisterUserDto dto);
    Task<string?> LoginAsync(LoginDto dto);
}
