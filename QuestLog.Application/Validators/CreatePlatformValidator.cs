using FluentValidation;
using QuestLog.Application.DTOs.Platforms;

namespace QuestLog.Application.Validators;

public class CreatePlatformValidator : AbstractValidator<CreatePlatformRequest>
{
    public CreatePlatformValidator()
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Length(2, 100);
    }
}
