using Microsoft.AspNetCore.Mvc;
using QuestLog.Application.Contracts.Services;
using QuestLog.Application.DTOs.Platforms;

namespace QuestLog.API.Controllers;

[Route("api/platforms")]
public class PlatformsController(IPlatformService platformService) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var platforms = await platformService.GetAllAsync();

        return Ok(platforms);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await platformService.GetByIdAsync(id);

        return result.IsSuccess
            ? Ok(result.Data)
            : Failure(result.ErrorType, result.ErrorMessage);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePlatformRequest request)
    {
        var result = await platformService.CreateAsync(request);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data)
            : Failure(result.ErrorType, result.ErrorMessage);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePlatformRequest request)
    {
        var result = await platformService.UpdateAsync(id, request);

        return result.IsSuccess
            ? NoContent()
            : Failure(result.ErrorType, result.ErrorMessage);
    }


    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await platformService.DeleteAsync(id);

        return result.IsSuccess
            ? NoContent()
            : Failure(result.ErrorType, result.ErrorMessage);
    }
}
