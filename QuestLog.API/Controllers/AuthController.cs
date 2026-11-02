using Microsoft.AspNetCore.Mvc;
using QuestLog.Application.Contracts.Services;
using QuestLog.Application.DTOs.Auth;

namespace QuestLog.API.Controllers;

[Route("api/auth")]
public class AuthController(IAuthService authService) : ApiControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var result = await authService.RegisterAsync(request);

        return result.IsSuccess
            ? Ok(result.Data)
            : Failure(result.ErrorType, result.ErrorMessage);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login (LoginRequest request)
    {
        var result = await authService.LoginAsync(request);

        return result.IsSuccess
            ? Ok(result.Data)
            : Failure(result.ErrorType, result.ErrorMessage);
    }
}
