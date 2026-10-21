namespace QuestLog.Application.DTOs.Platforms;

public sealed record PlatformResponse(
    Guid Id,
    string Name,
    string Slug
    );
