using Microsoft.AspNetCore.Identity;

namespace QuestLog.Domain.Entities;

public class AppRole : IdentityRole<Guid>
{
    public DateTimeOffset CreatedOn { get; set; }

    public AppRole()
    {
        var now = TimeProvider.System.GetUtcNow();
        Id = Guid.CreateVersion7(now);
        CreatedOn = now;
    }
}
