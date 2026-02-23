using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Syndicate.Domain.Entities;

namespace Syndicate.Infrastructure.Data.Configurations;

public class PollOptionConfiguration : IEntityTypeConfiguration<PollOption>
{
    public void Configure(EntityTypeBuilder<PollOption> builder)
    {
        builder.ToTable("poll_options");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Text)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne(o => o.Poll)
            .WithMany(p => p.Options)
            .HasForeignKey(o => o.PollId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
