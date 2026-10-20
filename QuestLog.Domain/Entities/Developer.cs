using QuestLog.Domain.Common;

namespace QuestLog.Domain.Entities;

public class Developer : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public ICollection<Game> Games { get; set; } = [];
}
