using Microsoft.EntityFrameworkCore;
using Syndicate.Domain.Entities;
using Syndicate.Domain.Interfaces;
using Syndicate.Infrastructure.Data;

namespace Syndicate.Infrastructure.Repositories;

public class MemberRepository : IMemberRepository
{
    private readonly SyndicateDbContext _context;

    public MemberRepository(SyndicateDbContext context)
    {
        _context = context;
    }

    public async Task<Member?> GetByIdAsync(Guid id)
        => await _context.Members
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.Id == id);

    public async Task<Member?> GetByUserAndGroupAsync(Guid userId, Guid groupId)
        => await _context.Members
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.UserId == userId && m.GroupId == groupId);

    public async Task<List<Member>> GetByGroupIdAsync(Guid groupId)
        => await _context.Members
            .Include(m => m.User)
            .Where(m => m.GroupId == groupId)
            .OrderBy(m => m.JoinedAt)
            .ToListAsync();

    public async Task<bool> ExistsAsync(Guid userId, Guid groupId)
        => await _context.Members.AnyAsync(m => m.UserId == userId && m.GroupId == groupId);

    public async Task AddAsync(Member member)
        => await _context.Members.AddAsync(member);
}
