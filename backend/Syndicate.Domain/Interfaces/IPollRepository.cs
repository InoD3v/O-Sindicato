using Syndicate.Domain.Entities;

namespace Syndicate.Domain.Interfaces;

public interface IPollRepository
{
    Task<Poll?> GetByIdWithOptionsAndVotesAsync(Guid id);
    Task<List<Poll>> GetByGroupIdAsync(Guid groupId);
    Task<bool> HasUserVotedAsync(Guid pollId, Guid memberId);
    Task AddAsync(Poll poll);
    Task AddVoteAsync(Vote vote);
}
