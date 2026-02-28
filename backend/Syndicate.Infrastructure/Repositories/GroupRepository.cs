using Microsoft.EntityFrameworkCore;
using Syndicate.Domain.Entities;
using Syndicate.Domain.Interfaces;
using Syndicate.Infrastructure.Data;

namespace Syndicate.Infrastructure.Repositories;

public class GroupRepository : IGroupRepository
{
    private readonly SyndicateDbContext _context;

    public GroupRepository(SyndicateDbContext context)
    {
        _context = context;
    }

    public async Task<Group?> GetByIdAsync(Guid id)
        => await _context.Groups
            .Include(g => g.Owner)
            .FirstOrDefaultAsync(g => g.Id == id);

    public async Task<Group?> GetByInviteCodeAsync(string inviteCode)
        => await _context.Groups
            .Include(g => g.Owner)
            .FirstOrDefaultAsync(g => g.InviteCode == inviteCode);

    public async Task<List<Group>> GetByUserIdAsync(Guid userId)
        => await _context.Groups
            .Where(g => g.Members.Any(m => m.UserId == userId))
            .ToListAsync();

    public async Task AddAsync(Group group)
        => await _context.Groups.AddAsync(group);
}
