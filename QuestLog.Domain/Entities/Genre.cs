using QuestLog.Domain.Common;

namespace QuestLog.Domain.Entities;

public class Genre : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    public ICollection<GameGenre> Games { get; set; } = [];
}
