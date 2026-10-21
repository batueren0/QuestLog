namespace QuestLog.Application.DTOs.Games;

public sealed record UpdateGameRequest(
    string Title,
    string Description,
    DateOnly ReleaseDate,
    string? CoverImageUrl,
    Guid DeveloperId,
    IReadOnlyList<Guid> GenreIds,
    IReadOnlyList<Guid> PlatformIds
    );
