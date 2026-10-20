using QuestLog.Domain.Common;
using QuestLog.Domain.Enums;

namespace QuestLog.Domain.Entities;

public class LibraryEntry : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid GameId { get; set; }
    public GameStatus Status { get; set; }
    public int? HoursPlayed { get; set; }

    public Game? Game { get; set; }
    public AppUser? User { get; set; }
}
