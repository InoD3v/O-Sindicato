using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Syndicate.Domain.Entities;

namespace Syndicate.Infrastructure.Data.Configurations;

public class VoteConfiguration : IEntityTypeConfiguration<Vote>
{
    public void Configure(EntityTypeBuilder<Vote> builder)
    {
        builder.ToTable("votes");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Weight)
            .HasPrecision(18, 2);

        // One vote per member per poll (enforced via unique index on PollOption's PollId + MemberId)
        builder.HasIndex(v => new { v.PollOptionId, v.MemberId });

        builder.HasOne(v => v.PollOption)
            .WithMany(o => o.Votes)
            .HasForeignKey(v => v.PollOptionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(v => v.Member)
            .WithMany()
            .HasForeignKey(v => v.MemberId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
