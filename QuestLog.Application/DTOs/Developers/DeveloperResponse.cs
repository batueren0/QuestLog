namespace QuestLog.Application.DTOs.Developers;

public sealed record DeveloperResponse(
    Guid Id,
    string Name,
    string Slug
    );
