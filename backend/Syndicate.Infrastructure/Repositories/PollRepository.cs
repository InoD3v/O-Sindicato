using Microsoft.EntityFrameworkCore;
using Syndicate.Domain.Entities;
using Syndicate.Domain.Interfaces;
using Syndicate.Infrastructure.Data;

namespace Syndicate.Infrastructure.Repositories;

public class PollRepository : IPollRepository
{
    private readonly SyndicateDbContext _context;

    public PollRepository(SyndicateDbContext context)
    {
        _context = context;
    }

    public async Task<Poll?> GetByIdWithOptionsAndVotesAsync(Guid id)
        => await _context.Polls
            .Include(p => p.Creator).ThenInclude(m => m.User)
            .Include(p => p.Options).ThenInclude(o => o.Votes)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<List<Poll>> GetByGroupIdAsync(Guid groupId)
        => await _context.Polls
            .Include(p => p.Creator).ThenInclude(m => m.User)
            .Include(p => p.Options).ThenInclude(o => o.Votes)
            .Where(p => p.GroupId == groupId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

    public async Task<bool> HasUserVotedAsync(Guid pollId, Guid memberId)
        => await _context.Votes
            .AnyAsync(v => v.PollOption.PollId == pollId && v.MemberId == memberId);

    public async Task AddAsync(Poll poll)
        => await _context.Polls.AddAsync(poll);

    public async Task AddVoteAsync(Vote vote)
        => await _context.Votes.AddAsync(vote);
}
