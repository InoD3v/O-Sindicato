using Microsoft.EntityFrameworkCore;
using Syndicate.Domain.Entities;
using Syndicate.Domain.Interfaces;
using Syndicate.Infrastructure.Data;

namespace Syndicate.Infrastructure.Repositories;

public class DebtRepository : IDebtRepository
{
    private readonly SyndicateDbContext _context;

    public DebtRepository(SyndicateDbContext context)
    {
        _context = context;
    }

    public async Task<Debt?> GetByIdAsync(Guid id)
        => await _context.Debts
            .Include(d => d.Creditor).ThenInclude(m => m.User)
            .Include(d => d.Debtor).ThenInclude(m => m.User)
            .FirstOrDefaultAsync(d => d.Id == id);

    public async Task<List<Debt>> GetByGroupIdAsync(Guid groupId)
        => await _context.Debts
            .Include(d => d.Creditor).ThenInclude(m => m.User)
            .Include(d => d.Debtor).ThenInclude(m => m.User)
            .Where(d => d.GroupId == groupId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();

    /// <summary>
    /// Sum of all active (escrowed) debt amounts where the member is the debtor.
    /// </summary>
    public async Task<decimal> GetBlockedBalanceAsync(Guid debtorMemberId)
        => await _context.Debts
            .Where(d => d.DebtorMemberId == debtorMemberId && d.Status == DebtStatus.Active)
            .SumAsync(d => d.Amount);

    public async Task AddAsync(Debt debt)
        => await _context.Debts.AddAsync(debt);

    public void Update(Debt debt)
        => _context.Debts.Update(debt);
}
