using Syndicate.Domain.Entities;

namespace Syndicate.Domain.Interfaces;

public interface IGroupRepository
{
    Task<Group?> GetByIdAsync(Guid id);
    Task<Group?> GetByInviteCodeAsync(string inviteCode);
    Task<List<Group>> GetByUserIdAsync(Guid userId);
    Task AddAsync(Group group);
}
