using QuestLog.Application.Common;
using QuestLog.Application.DTOs.Auth;

namespace QuestLog.Application.Contracts.Services;

public interface IAuthService
{
    Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request);
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request);
}
