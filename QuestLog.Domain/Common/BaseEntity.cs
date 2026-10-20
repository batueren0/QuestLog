
namespace QuestLog.Domain.Common;

public abstract class BaseEntity
{

    public Guid Id { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset LastModifiedOn { get; set; }
    public DateTimeOffset? DeletedOn { get; set; }

    protected BaseEntity()
    {
        var now = TimeProvider.System.GetUtcNow();

        Id = Guid.CreateVersion7(now);
        IsActive = true;
        CreatedOn = now;
        LastModifiedOn = now;
    }
}
