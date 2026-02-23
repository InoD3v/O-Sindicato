using Syndicate.Domain.Entities;

namespace Syndicate.Domain.Interfaces;

public interface ITransactionRepository
{
    Task<decimal> GetBalanceAsync(Guid memberId);
    Task AddAsync(Transaction transaction);
}
