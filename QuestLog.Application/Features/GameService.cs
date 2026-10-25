using FluentValidation;
using QuestLog.Application.Common;
using QuestLog.Application.Contracts.Services;
using QuestLog.Application.DTOs.Developers;
using QuestLog.Application.DTOs.Games;
using QuestLog.Application.DTOs.Genres;
using QuestLog.Application.DTOs.Platforms;
using QuestLog.Domain.Entities;

namespace QuestLog.Application.Features;

public class GameService(IUnitOfWork uow,
    IValidator<CreateGameRequest> createValidator,
    IValidator<UpdateGameRequest> updateValidator
    ) : IGameService
{
    public async Task<Result<GameDetailResponse>> CreateAsync(CreateGameRequest request)
    {
        var validationResult = await createValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return Result<GameDetailResponse>.Failure(ErrorType.Validation, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));


        var referenceError = await ValidateReferencesAsync(request.DeveloperId, request.GenreIds, request.PlatformIds);
        if (referenceError is not null)
            return Result<GameDetailResponse>.Failure(ErrorType.Validation, referenceError);

        var slug = request.Title.ToKebabCase();
        var slugExists = await uow.Games.SlugExistsAsync(slug);
        if (slugExists)
            return Result<GameDetailResponse>.Failure(ErrorType.Conflict, "A game with this title already exists.");

        var game = new Game
        {
            Title = request.Title,
            Slug = slug,
            Description = request.Description,
            ReleaseDate = request.ReleaseDate,
            CoverImageUrl = request.CoverImageUrl,
            DeveloperId = request.DeveloperId,

            Genres = request.GenreIds.Select(id => new GameGenre { GenreId = id }).ToList(),
            Platforms = request.PlatformIds.Select(id => new GamePlatform { PlatformId = id}).ToList(),

        };


        await uow.Games.AddAsync(game);
        await uow.SaveChangesAsync();

        var createdGame = await uow.Games.GetBySlugWithDetailsAsync(slug);
        return Result<GameDetailResponse>.Success(ToDetailResponse(createdGame!));

    }

    public async Task<Result> UpdateAsync(Guid id, UpdateGameRequest request)
    {

        var validationResult = await updateValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return Result.Failure(ErrorType.Validation, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

        var game = await uow.Games.GetByIdWithRelationsAsync(id);
        if (game is null)
            return Result.Failure(ErrorType.NotFound, "Game not found.");

        var referencesError = await ValidateReferencesAsync(request.DeveloperId, request.GenreIds, request.PlatformIds);
        if (referencesError is not null)
            return Result.Failure(ErrorType.Validation, referencesError);

        var slug = request.Title.ToKebabCase();
        var slugExists = await uow.Games.SlugExistsAsync(slug, id);
        if (slugExists)
            return Result.Failure(ErrorType.Conflict, "A game with this title already exists.");

        game.Title = request.Title;
        game.Slug = slug;
        game.Description = request.Description;
        game.ReleaseDate = request.ReleaseDate;
        game.CoverImageUrl = request.CoverImageUrl;
        game.DeveloperId = request.DeveloperId;

        var genresToRemove = game.Genres
            .Where(gg => !request.GenreIds.Contains(gg.GenreId))
            .ToList();

        foreach(var gameGenre in genresToRemove)
            game.Genres.Remove(gameGenre);

        var genreIdsToAdd = request.GenreIds
            .Where(genreId => !game.Genres.Any(gg => gg.GenreId == genreId))
            .ToList();

        foreach (var genreId in genreIdsToAdd)
            game.Genres.Add(new GameGenre { GenreId = genreId });

        var platformsToRemove = game.Platforms
            .Where(gp => !request.PlatformIds.Contains(gp.PlatformId))
            .ToList();

        foreach(var gamePlatform in platformsToRemove)
            game.Platforms.Remove(gamePlatform);

        var platformIdsToAdd = request.PlatformIds
            .Where(platformId => !game.Platforms.Any(gp => gp.PlatformId == platformId))
            .ToList();

        foreach (var platformId in platformIdsToAdd)
            game.Platforms.Add(new GamePlatform { PlatformId = platformId});


        uow.Games.Update(game);
        await uow.SaveChangesAsync();

        return Result.Success();

    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var game = await uow.Games.GetByIdAsync(id);
        if (game is null)
            return Result.Failure(ErrorType.NotFound, "Game not found.");

        uow.Games.Delete(game);
        await uow.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result<GameDetailResponse>> GetBySlugAsync(string slug)
    {
        var game = await uow.Games.GetBySlugWithDetailsAsync(slug);
        if (game is null)
            return Result<GameDetailResponse>.Failure(ErrorType.NotFound, "Game not found.");

        return Result<GameDetailResponse>.Success(ToDetailResponse(game));
    }

    public async Task<PagedResult<GameSummaryResponse>> GetPagedAsync(int page, int pageSize, string? search, Guid? genreId, Guid? platformId)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 50);

        var pagedGames = await uow.Games.GetPagedAsync(page, pageSize, search, genreId, platformId);

        var items = pagedGames.Items
            .Select(ToSummaryResponse).ToList();

        return new PagedResult<GameSummaryResponse>
        {
            Items = items,
            Page = pagedGames.Page,
            PageSize = pagedGames.PageSize,
            TotalCount = pagedGames.TotalCount
        };
    }

    private static GameSummaryResponse ToSummaryResponse(Game game) => new(
        game.Id,
        game.Title,
        game.Slug,
        game.ReleaseDate,
        game.CoverImageUrl,
        game.Developer!.Name,
        game.Genres.Select(gg => gg.Genre!.Name).ToList()
        );

    private static GameDetailResponse ToDetailResponse(Game game) => new(
        game.Id,
        game.Title,
        game.Slug,
        game.Description,
        game.ReleaseDate,
        game.CoverImageUrl,
        new DeveloperResponse(game.Developer!.Id, game.Developer.Name, game.Developer.Slug),
        game.Genres.Select(gg => new GenreResponse(gg.Genre!.Id, gg.Genre.Name, gg.Genre.Slug)).ToList(),
        game.Platforms.Select(gp => new PlatformResponse(gp.Platform!.Id, gp.Platform.Name, gp.Platform.Slug)).ToList()
        );

    private async Task<string?> ValidateReferencesAsync(Guid developerId, IReadOnlyList<Guid> genreIds, IReadOnlyList<Guid> platformIds)
    {
        var developer = await uow.Developers.GetByIdAsync(developerId);
        if (developer is null)
            return "Developer not found.";


        var existingGenreCount = await uow.Genres.CountExistingAsync(genreIds);
        if (existingGenreCount != genreIds.Count)
            return "One or more genres were not found.";


        var existingPlatformCount = await uow.Platforms.CountExistingAsync(platformIds);
        if (existingPlatformCount != platformIds.Count)
            return "One or more platforms were not found.";

        return null;
    }
}
