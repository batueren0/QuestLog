using FluentValidation;
using QuestLog.Application.DTOs.Developers;

namespace QuestLog.Application.Validators;

public class CreateDeveloperValidator : AbstractValidator<CreateDeveloperRequest>
{
    public CreateDeveloperValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(2, 100);
    }
}
