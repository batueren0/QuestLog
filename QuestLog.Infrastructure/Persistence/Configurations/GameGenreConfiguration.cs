using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuestLog.Domain.Entities;

namespace QuestLog.Infrastructure.Persistence.Configurations;

public class GameGenreConfiguration : BaseEntityConfiguration<GameGenre>
{
    protected override void ConfigureEntity(EntityTypeBuilder<GameGenre> builder)
    {
        builder.HasOne(x => x.Game)
            .WithMany(x => x.Genres)
            .HasForeignKey(x => x.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Genre)
            .WithMany(x => x.Games)
            .HasForeignKey(x => x.GenreId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.GameId, x.GenreId })
            .IsUnique()
            .HasFilter("[IsActive] = 1");
    }
}
