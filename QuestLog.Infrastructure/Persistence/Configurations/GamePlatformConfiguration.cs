using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuestLog.Domain.Entities;

namespace QuestLog.Infrastructure.Persistence.Configurations;

public class GamePlatformConfiguration : BaseEntityConfiguration<GamePlatform>
{
    protected override void ConfigureEntity(EntityTypeBuilder<GamePlatform> builder)
    {
        builder.HasOne(x => x.Game)
            .WithMany(x => x.Platforms)
            .HasForeignKey(x => x.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Platform)
            .WithMany(x => x.Games)
            .HasForeignKey(x => x.PlatformId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.GameId, x.PlatformId })
            .IsUnique()
            .HasFilter("[IsActive] = 1");
    }
}
