using Microsoft.AspNetCore.Mvc;
using QuestLog.Application.Contracts.Services;
using QuestLog.Application.DTOs.Games;

namespace QuestLog.API.Controllers;

[Route("api/games")]
public class GamesController(IGameService gameService) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] Guid? genreId = null,
        [FromQuery] Guid? platformId = null
        )
    {
        var games = await gameService.GetPagedAsync(page, pageSize, search, genreId, platformId);

        return Ok(games);
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var result = await gameService.GetBySlugAsync(slug);

        return result.IsSuccess
            ? Ok(result.Data)
            : Failure(result.ErrorType, result.ErrorMessage);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateGameRequest request)
    {
        var result = await gameService.CreateAsync(request);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetBySlug), new { slug = result.Data!.Slug }, result.Data)
            : Failure(result.ErrorType, result.ErrorMessage);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id ,UpdateGameRequest request)
    {
        var result = await gameService.UpdateAsync(id, request);

        return result.IsSuccess
            ? NoContent()
            : Failure(result.ErrorType, result.ErrorMessage);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await gameService.DeleteAsync(id);

        return result.IsSuccess
            ? NoContent()
            : Failure(result.ErrorType, result.ErrorMessage);
    }
}
