using Microsoft.AspNetCore.Mvc;
using QuestLog.Application.Common;

namespace QuestLog.API.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult Failure(ErrorType? errorType, string message)
    {
        var statusCode = errorType switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _                 => StatusCodes.Status500InternalServerError,
        };

        return Problem(statusCode: statusCode, detail: message);
    }
}
