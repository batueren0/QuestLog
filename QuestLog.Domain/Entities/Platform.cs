using QuestLog.Domain.Common;

namespace QuestLog.Domain.Entities;

public class Platform : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    public ICollection<GamePlatform> Games { get; set; } = [];
}
