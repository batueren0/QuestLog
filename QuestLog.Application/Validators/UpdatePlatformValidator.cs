using FluentValidation;
using QuestLog.Application.DTOs.Platforms;

namespace QuestLog.Application.Validators;

public class UpdatePlatformValidator : AbstractValidator<UpdatePlatformRequest>
{
    public UpdatePlatformValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(2, 100);
    }
}
