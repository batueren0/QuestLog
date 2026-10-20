using QuestLog.Domain.Common;

namespace QuestLog.Domain.Entities;

public class Game : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set;  } = string.Empty;
    public DateOnly ReleaseDate { get; set; }
    public string? CoverImageUrl { get; set; }
    public Guid DeveloperId { get; set; }


    public ICollection<GameGenre> Genres { get; set; } = [];
    public ICollection<GamePlatform> Platforms { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
    public ICollection<LibraryEntry> LibraryEntries { get; set; } = [];
    public Developer? Developer { get; set; }

}
