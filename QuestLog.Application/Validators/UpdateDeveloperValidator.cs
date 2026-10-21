using FluentValidation;
using QuestLog.Application.DTOs.Developers;

namespace QuestLog.Application.Validators;

public class UpdateDeveloperValidator : AbstractValidator<UpdateDeveloperRequest>
{
    public UpdateDeveloperValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(2, 100);
    }
}
