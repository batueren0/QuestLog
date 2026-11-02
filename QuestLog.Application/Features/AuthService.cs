using FluentValidation;
using Microsoft.AspNetCore.Identity;
using QuestLog.Application.Common;
using QuestLog.Application.Contracts.Services;
using QuestLog.Application.DTOs.Auth;
using QuestLog.Domain.Entities;

namespace QuestLog.Application.Features;

public class AuthService(
    UserManager<AppUser> userManager,
    ITokenService tokenService,
    IValidator<RegisterRequest> registerValidator,
    IValidator<LoginRequest> loginValidator) : IAuthService
{
    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var validatorResult = await loginValidator.ValidateAsync(request);
        if (!validatorResult.IsValid)
            return Result<AuthResponse>.Failure(ErrorType.Validation, string.Join(", ", validatorResult.Errors.Select(e => e.ErrorMessage)));

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Result<AuthResponse>.Failure(ErrorType.Unauthorized, "Invalid email or password.");
        var isPasswordValid = await userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
            return Result<AuthResponse>.Failure(ErrorType.Unauthorized, "Invalid email or password.");

        var roles = await userManager.GetRolesAsync(user);
        var token = tokenService.CreateToken(user, roles);
        return Result<AuthResponse>.Success(ToResponse(user, token, roles));
    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        var validatorResult = await registerValidator.ValidateAsync(request);
        if (!validatorResult.IsValid)
            return Result<AuthResponse>.Failure(ErrorType.Validation, string.Join(", ", validatorResult.Errors.Select(e => e.ErrorMessage)));

        var userWithEmail = await userManager.FindByEmailAsync(request.Email);
        if (userWithEmail is not null)
            return Result<AuthResponse>.Failure(ErrorType.Conflict, "An account with this email already exists.");

        var user = new AppUser
        {
            UserName = request.Email,
            Email = request.Email,
            DisplayName = request.DisplayName,
        };

        var createResult = await userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
            return Result<AuthResponse>.Failure(ErrorType.Validation, string.Join(", ", createResult.Errors.Select(e => e.Description)));

        var roles = await userManager.GetRolesAsync(user);
        var token = tokenService.CreateToken(user, roles);
        return Result<AuthResponse>.Success(ToResponse(user, token, roles));
    }

    private static AuthResponse ToResponse(AppUser user, AccessToken token, IList<string> roles) =>
    new(token.Token, token.ExpiresAt, user.Id, user.Email!, user.DisplayName, roles.ToList());
}
