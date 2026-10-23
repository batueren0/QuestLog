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
            return Result<GameDetailResponse>.Failure(ErrorType.Validation,string.Join(", ",validationResult.Errors.Select(e => e.ErrorMessage)));


        var developer = await uow.Developers.GetByIdAsync(request.DeveloperId);
        if (developer is null)
            return Result<GameDetailResponse>.Failure(ErrorType.Validation, "Developer not found.");


        var existingGenreCount = await uow.Genres.CountExistingAsync(request.GenreIds);
        if (existingGenreCount != request.GenreIds.Count)
            return Result<GameDetailResponse>.Failure(ErrorType.Validation, "One or more genres were not found.");


        var existingPlatformCount = await uow.Platforms.CountExistingAsync(request.PlatformIds);
        if (existingPlatformCount != request.PlatformIds.Count)
            return Result<GameDetailResponse>.Failure(ErrorType.Validation, "One or more platforms were not found.");

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

    public Task<Result> DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
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

    public Task<Result> UpdateAsync(Guid id, UpdateGameRequest request)
    {
        throw new NotImplementedException();
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
}
