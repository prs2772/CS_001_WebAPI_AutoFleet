using AutoFleet.Core.Entities;

namespace AutoFleet.Core.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<bool> UserExistsAsync(string username);
    Task AddAsync(User user);
}
