using FluentValidation;
using QuestLog.Application.DTOs.Genres;

namespace QuestLog.Application.Validators;

public class CreateGenreValidator : AbstractValidator<CreateGenreRequest>
{
    public CreateGenreValidator()
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Length(2, 100);
    }
}
