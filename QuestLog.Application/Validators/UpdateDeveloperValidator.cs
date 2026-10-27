using FluentValidation;
using QuestLog.Application.DTOs.Developers;

namespace QuestLog.Application.Validators;

public class UpdateDeveloperValidator : AbstractValidator<UpdateDeveloperRequest>
{
    public UpdateDeveloperValidator()
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Length(2, 100);
    }
}
