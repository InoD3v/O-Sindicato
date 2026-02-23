using Microsoft.EntityFrameworkCore;
using Syndicate.Domain.Entities;
using Syndicate.Domain.Interfaces;
using Syndicate.Infrastructure.Data;

namespace Syndicate.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly SyndicateDbContext _context;

    public UserRepository(SyndicateDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
        => await _context.Users.FindAsync(id);

    public async Task<User?> GetByEmailAsync(string email)
        => await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User?> GetByUsernameAsync(string username)
        => await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

    public async Task<bool> ExistsByEmailAsync(string email)
        => await _context.Users.AnyAsync(u => u.Email == email);

    public async Task<bool> ExistsByUsernameAsync(string username)
        => await _context.Users.AnyAsync(u => u.Username == username);

    public async Task AddAsync(User user)
        => await _context.Users.AddAsync(user);
}
