using Syndicate.Domain.Entities;

namespace Syndicate.Domain.Interfaces;

public interface IMemberRepository
{
    Task<Member?> GetByIdAsync(Guid id);
    Task<Member?> GetByUserAndGroupAsync(Guid userId, Guid groupId);
    Task<List<Member>> GetByGroupIdAsync(Guid groupId);
    Task<bool> ExistsAsync(Guid userId, Guid groupId);
    Task AddAsync(Member member);
}
