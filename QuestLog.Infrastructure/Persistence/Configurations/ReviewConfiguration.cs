using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuestLog.Domain.Entities;

namespace QuestLog.Infrastructure.Persistence.Configurations;

public class ReviewConfiguration : BaseEntityConfiguration<Review>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Review> builder)
    {
        builder.Property(x => x.Body)
            .IsRequired(false)
            .HasMaxLength(2000);

        builder.HasOne(x => x.Game)
            .WithMany(x => x.Reviews)
            .HasForeignKey(x => x.GameId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Reviews)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.GameId, x.UserId })
            .IsUnique()
            .HasFilter("[IsActive] = 1");

        builder.ToTable(t => t.HasCheckConstraint("CK_Reviews_Rating", "[Rating] BETWEEN 1 AND 10"));
    }
}
