using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuestLog.Domain.Entities;

namespace QuestLog.Infrastructure.Persistence.Configurations;

public class PlatformConfiguration : BaseEntityConfiguration<Platform>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Platform> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(x => x.Slug)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.Name)
            .IsUnique()
            .HasFilter("[IsActive] = 1");
        builder.HasIndex(x => x.Slug)
            .IsUnique()
            .HasFilter("[IsActive] = 1");

           
    }
}
