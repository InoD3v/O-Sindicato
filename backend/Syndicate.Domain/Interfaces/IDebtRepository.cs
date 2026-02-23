using Syndicate.Domain.Entities;

namespace Syndicate.Domain.Interfaces;

public interface IDebtRepository
{
    Task<Debt?> GetByIdAsync(Guid id);
    Task<List<Debt>> GetByGroupIdAsync(Guid groupId);
    Task<decimal> GetBlockedBalanceAsync(Guid debtorMemberId);
    Task AddAsync(Debt debt);
    void Update(Debt debt);
}
