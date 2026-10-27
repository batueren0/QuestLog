using Microsoft.AspNetCore.Mvc;
using QuestLog.Application.Contracts.Services;
using QuestLog.Application.DTOs.Genres;

namespace QuestLog.API.Controllers;

[Route("api/genres")]
public class GenresController(IGenreService genreService) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var genres = await genreService.GetAllAsync();

        return Ok(genres);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await genreService.GetByIdAsync(id);

        return result.IsSuccess
            ? Ok(result.Data)
            : Failure(result.ErrorType, result.ErrorMessage);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGenreRequest request)
    {
        var result = await genreService.CreateAsync(request);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data)
            : Failure(result.ErrorType ,result.ErrorMessage);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id ,[FromBody] UpdateGenreRequest request)
    {
        var result = await genreService.UpdateAsync(id, request);

        return result.IsSuccess
            ? NoContent()
            : Failure(result.ErrorType, result.ErrorMessage);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await genreService.DeleteAsync(id);

        return result.IsSuccess
            ? NoContent()
            : Failure(result.ErrorType, result.ErrorMessage);
    }

}
