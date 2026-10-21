using FluentValidation;
using QuestLog.Application.DTOs.Games;

namespace QuestLog.Application.Validators;

public class UpdateGameValidator : AbstractValidator<UpdateGameRequest>
{
    public UpdateGameValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty()
            .Length(20, 4000);

        RuleFor(x => x.ReleaseDate)
            .GreaterThanOrEqualTo(new DateOnly(1970, 1, 1))
            .Must(date => date <= DateOnly.FromDateTime(DateTime.UtcNow).AddYears(5))
            .WithMessage("Release date cannot be more than 5 years in the future.");

        RuleFor(x => x.CoverImageUrl)
            .Must(BeAValidUrl)
            .WithMessage("Cover image URL must be a valid http or https address")
            .When(x => !string.IsNullOrEmpty(x.CoverImageUrl));

        RuleFor(x => x.DeveloperId)
            .NotEmpty();

        RuleFor(x => x.GenreIds)
            .NotEmpty()
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("Genre list cannot contain duplicate ids");

        RuleFor(x => x.PlatformIds)
            .NotEmpty()
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("Platfotm list cannot contain duplicate ids");

    }

    private bool BeAValidUrl(string? url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}
