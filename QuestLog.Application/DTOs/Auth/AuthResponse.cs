namespace QuestLog.Application.DTOs.Auth;

public sealed record AuthResponse(
    string Token,
    DateTimeOffset ExpiresAt,
    Guid UserId,
    string Email,
    string? DisplayName,
    IReadOnlyList<string> Roles
    );
