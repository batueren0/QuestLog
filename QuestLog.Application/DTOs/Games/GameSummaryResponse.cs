namespace QuestLog.Application.DTOs.Games;

public sealed record GameSummaryResponse(
    Guid Id,
    string Title,
    string Slug,
    DateOnly ReleaseDate,
    string? CoverImageUrl,
    string DeveloperName,
    IReadOnlyList<string> Genres
    );
