using AutoFleet.Core.Entities;
using AutoFleet.Core.Interfaces;
using AutoFleet.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AutoFleet.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AutoFleetDbContext _context;

    public UserRepository(AutoFleetDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<bool> UserExistsAsync(string username)
    {
        return await _context.Users.AnyAsync(u => u.Username == username);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }
}
