using FluentValidation;
using QuestLog.Application.Common;
using QuestLog.Application.Contracts.Services;
using QuestLog.Application.DTOs.Genres;
using QuestLog.Domain.Entities;


namespace QuestLog.Application.Features;

public class GenreService(
    IUnitOfWork uow, 
    IValidator<CreateGenreRequest> createValidator,
    IValidator<UpdateGenreRequest> updateValidator
    ) : IGenreService
{
    public async Task<Result<GenreResponse>> CreateAsync(CreateGenreRequest request)
    {
        var validationResult = await createValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return Result<GenreResponse>.Failure(ErrorType.Validation ,string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

        if (await uow.Genres.NameExistsAsync(request.Name))
            return Result<GenreResponse>.Failure(ErrorType.Conflict, "A genre with this name already exists.");


        var genre = new Genre
        {
            Name = request.Name,
            Slug = request.Name.ToKebabCase()
        };
        await uow.Genres.AddAsync(genre);
        await uow.SaveChangesAsync();

        return Result<GenreResponse>.Success(ToResponse(genre));
    }

    public async Task<Result> UpdateAsync(Guid id, UpdateGenreRequest request)
    {
        // 1) İstediğin biçim bozuk mu? > 400
        var validationResult = await updateValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return Result.Failure(ErrorType.Validation, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

        // 2) Güncellenecek kayıt var mı? > 404
        var genre = await uow.Genres.GetByIdAsync(id);
        if (genre is null)
            return Result.Failure(ErrorType.NotFound, "Genre not found.");

        // 3) yeni isim başka bir türde kullanılıyor mu? > 409
        if (await uow.Genres.NameExistsAsync(request.Name, id))
            return Result.Failure(ErrorType.Conflict, "A genre with this name already exists.");

        // 4) bütün kontrollar geçti: değiştir ve kaydet.
        genre.Name = request.Name;
        genre.Slug = request.Name.ToKebabCase();

        uow.Genres.Update(genre);
        await uow.SaveChangesAsync();

        return Result.Success();

    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var genre = await uow.Genres.GetByIdAsync(id);
        if (genre is null)
            return Result.Failure(ErrorType.NotFound, "Genre not found.");

        if (await uow.Genres.HasGamesAsync(id))
            return Result.Failure(ErrorType.Conflict, "This genre has games and cannot be deleted.");

        uow.Genres.Delete(genre);
        await uow.SaveChangesAsync();
        return Result.Success();
        
    }

    public async Task<IReadOnlyList<GenreResponse>> GetAllAsync()
    {
        var genres = await uow.Genres.GetAllAsync();

        return genres.Select(ToResponse).ToList();
    }

    public async Task<Result<GenreResponse>> GetByIdAsync(Guid id)
    {
        var genre = await uow.Genres.GetByIdAsync(id);
        if (genre is null)
            return Result<GenreResponse>.Failure(ErrorType.NotFound, "Genre not found.");

        return Result<GenreResponse>.Success(ToResponse(genre));
    }




    private static GenreResponse ToResponse(Genre genre) => new(genre.Id, genre.Name, genre.Slug);

}
