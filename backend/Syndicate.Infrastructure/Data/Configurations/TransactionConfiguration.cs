using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Syndicate.Domain.Entities;

namespace Syndicate.Infrastructure.Data.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("transactions");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Amount)
            .HasPrecision(18, 2);

        builder.Property(t => t.Type)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(t => t.Description)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne(t => t.Member)
            .WithMany(m => m.Transactions)
            .HasForeignKey(t => t.MemberId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Debt)
            .WithMany()
            .HasForeignKey(t => t.DebtId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
