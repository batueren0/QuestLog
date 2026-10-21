namespace QuestLog.Application.DTOs.Genres;

public sealed record GenreResponse(
    Guid Id,
    string Name,
    string Slug
    );
