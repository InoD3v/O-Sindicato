using Microsoft.EntityFrameworkCore;
using Syndicate.Domain.Entities;
using Syndicate.Domain.Interfaces;
using Syndicate.Infrastructure.Data;

namespace Syndicate.Infrastructure.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly SyndicateDbContext _context;

    public TransactionRepository(SyndicateDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Ledger balance = SUM(Amount) for a given member.
    /// </summary>
    public async Task<decimal> GetBalanceAsync(Guid memberId)
        => await _context.Transactions
            .Where(t => t.MemberId == memberId)
            .SumAsync(t => t.Amount);

    public async Task AddAsync(Transaction transaction)
        => await _context.Transactions.AddAsync(transaction);
}
