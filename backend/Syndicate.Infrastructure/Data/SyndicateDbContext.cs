using Microsoft.EntityFrameworkCore;
using Syndicate.Domain.Entities;
using Syndicate.Domain.Interfaces;

namespace Syndicate.Infrastructure.Data;

public class SyndicateDbContext : DbContext, IUnitOfWork
{
    public SyndicateDbContext(DbContextOptions<SyndicateDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Debt> Debts => Set<Debt>();
    public DbSet<Poll> Polls => Set<Poll>();
    public DbSet<PollOption> PollOptions => Set<PollOption>();
    public DbSet<Vote> Votes => Set<Vote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SyndicateDbContext).Assembly);
    }
}
