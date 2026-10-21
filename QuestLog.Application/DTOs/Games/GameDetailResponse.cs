using QuestLog.Application.DTOs.Developers;
using QuestLog.Application.DTOs.Genres;
using QuestLog.Application.DTOs.Platforms;

namespace QuestLog.Application.DTOs.Games;

public sealed record GameDetailResponse(
    Guid Id,
    string Title,
    string Slug,
    string Description,
    DateOnly ReleaseDate,
    string? CoverImageUrl,
    DeveloperResponse Developer,
    IReadOnlyList<GenreResponse> Genres,
    IReadOnlyList<PlatformResponse> Platforms
    );
