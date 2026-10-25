using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuestLog.Domain.Entities;

namespace QuestLog.Infrastructure.Persistence.Configurations;

public class LibraryEntryConfiguration : BaseEntityConfiguration<LibraryEntry>
{
    protected override void ConfigureEntity(EntityTypeBuilder<LibraryEntry> builder)
    {
        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasOne(x => x.Game)
            .WithMany(x => x.LibraryEntries)
            .HasForeignKey(x => x.GameId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithMany(x => x.LibraryEntries)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.UserId, x.GameId })
            .IsUnique()
            .HasFilter("[IsActive] = 1");
    }
}
