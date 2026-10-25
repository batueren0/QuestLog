using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuestLog.Domain.Entities;

namespace QuestLog.Infrastructure.Persistence.Configurations;

public class GameConfiguration : BaseEntityConfiguration<Game>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Game> builder)
    {
        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(x => x.Slug)
            .IsRequired()
            .HasMaxLength(200);
        builder.HasIndex(x => x.Slug)
            .IsUnique()
            .HasFilter("[IsActive] = 1");
        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(4000);
        builder.Property(x => x.CoverImageUrl)
            .IsRequired(false)
            .HasMaxLength(500);

        builder.HasOne(x => x.Developer)
            .WithMany(x => x.Games)
            .HasForeignKey(x => x.DeveloperId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
