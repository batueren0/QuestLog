using Microsoft.AspNetCore.Mvc;
using QuestLog.Application.Contracts.Services;
using QuestLog.Application.DTOs.Developers;

namespace QuestLog.API.Controllers;

[Route("api/developers")]
public class DevelopersController(IDeveloperService developerService) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var developers = await developerService.GetAllAsync();

        return Ok(developers);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await developerService.GetByIdAsync(id);

        return result.IsSuccess
            ? Ok(result.Data)
            : Failure(result.ErrorType, result.ErrorMessage);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDeveloperRequest request)
    {
        var result = await developerService.CreateAsync(request);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data)
            : Failure(result.ErrorType, result.ErrorMessage);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDeveloperRequest request)
    {
        var result = await developerService.UpdateAsync(id, request);

        return result.IsSuccess
            ? NoContent()
            : Failure(result.ErrorType, result.ErrorMessage);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await developerService.DeleteAsync(id);

        return result.IsSuccess
            ? NoContent()
            : Failure(result.ErrorType, result.ErrorMessage);
    }
}
