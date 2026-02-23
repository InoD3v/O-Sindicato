using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Syndicate.Domain.Entities;

namespace Syndicate.Infrastructure.Data.Configurations;

public class DebtConfiguration : IEntityTypeConfiguration<Debt>
{
    public void Configure(EntityTypeBuilder<Debt> builder)
    {
        builder.ToTable("debts");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Amount)
            .HasPrecision(18, 2);

        builder.Property(d => d.Description)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(d => d.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasOne(d => d.Group)
            .WithMany(g => g.Debts)
            .HasForeignKey(d => d.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Creditor)
            .WithMany()
            .HasForeignKey(d => d.CreditorMemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Debtor)
            .WithMany()
            .HasForeignKey(d => d.DebtorMemberId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
