using FluentValidation;
using QuestLog.Application.DTOs.Auth;

namespace QuestLog.Application.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(256)
            .EmailAddress();
        RuleFor(x => x.Password)
            .NotEmpty();
        RuleFor(x => x.DisplayName)
            .NotEmpty()
            .Length(2, 50);
    }
}
