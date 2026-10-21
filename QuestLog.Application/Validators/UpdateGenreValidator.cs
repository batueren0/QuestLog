using FluentValidation;
using QuestLog.Application.DTOs.Genres;

namespace QuestLog.Application.Validators;

public class UpdateGenreValidator : AbstractValidator<UpdateGenreRequest>
{
    public UpdateGenreValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(2, 100);
    }
}
