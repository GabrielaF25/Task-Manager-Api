using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Common.ResultPattern;

namespace Task_Manager_Api.Controllers;

public abstract class BaseController : ControllerBase
{
    protected ActionResult<T> HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }

        return CreateProblemDetails(result);
    }

    protected ActionResult<T> HandleCreatedResult<T>(string route,Result<T> result, Func<T, object> routeValues)
    {
        if (result.IsSuccess)
        {
            return CreatedAtRoute(route, routeValues(result.Data!), result.Data);

        }

        return CreateProblemDetails(result);
    }

    protected ActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
        {
            return NoContent();
        }

        return CreateProblemDetails(result);

    }

    private ActionResult CreateProblemDetails(Result result)
    {
        var statusCode = result.StatusType switch
        {
            StatusType.NotFound => StatusCodes.Status404NotFound,
            StatusType.ValidationError => StatusCodes.Status400BadRequest,
            StatusType.Conflict => StatusCodes.Status409Conflict,
            StatusType.Unauthorized => StatusCodes.Status401Unauthorized,
            StatusType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status400BadRequest
        };

        var title = result.StatusType switch
        {
            StatusType.NotFound => "Resource not found",
            StatusType.ValidationError => "Validation failed",
            StatusType.Conflict => "Conflict",
            StatusType.Unauthorized => "Unauthorized",
            StatusType.Forbidden => "Forbidden",
            _ => "An error occurred"
        };

        var problemDetails = new ProblemDetails()
        {
            Title = title,
            Detail = result.Errors.FirstOrDefault(),
            Instance = HttpContext.Request.Path,
            Status = statusCode
        };

        problemDetails.Extensions["errors"] = result.Errors;

        return StatusCode(statusCode, problemDetails);
    }
}
