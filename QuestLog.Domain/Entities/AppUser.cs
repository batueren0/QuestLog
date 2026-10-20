using Microsoft.AspNetCore.Identity;

namespace QuestLog.Domain.Entities;

public class AppUser : IdentityUser<Guid>
{
    public string? DisplayName { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public ICollection<Review> Reviews { get; set; } = [];
    public ICollection<LibraryEntry> LibraryEntries { get; set; } = [];


    public AppUser()
    {
        var now = TimeProvider.System.GetUtcNow();
        Id = Guid.CreateVersion7(now);
        CreatedOn = now;
        SecurityStamp = Guid.CreateVersion7(now).ToString();
    }
}
