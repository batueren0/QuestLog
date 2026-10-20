using QuestLog.Domain.Common;

namespace QuestLog.Domain.Entities;

public class Review : BaseEntity
{
    public Guid GameId { get; set; }
    public Guid UserId { get; set; }
    public int Rating { get; set; }
    public string? Body { get; set; }

    public Game? Game { get; set; }
    public AppUser? User { get; set; }
}
