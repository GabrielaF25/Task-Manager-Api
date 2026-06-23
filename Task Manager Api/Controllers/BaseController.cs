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

        return result.StatusType switch
        {
            StatusType.NotFound => NotFound(result.Errors),
            StatusType.ValidationError => BadRequest(result.Errors),
            StatusType.Conflict => Conflict(result.Errors),
            _ => BadRequest(result.Errors)
        };
    }

    protected ActionResult<T> HandleCreatedResult<T>(string route,Result<T> result, Func<T, object> routeValues)
    {
        if (result.IsSuccess)
        {
            return CreatedAtRoute(route, new { routeValues },result.Data);
        }

        return result.StatusType switch
        {
            StatusType.NotFound => NotFound(result.Errors),
            StatusType.ValidationError => BadRequest(result.Errors),
            StatusType.Conflict => Conflict(result.Errors),
            _ => BadRequest(result.Errors)
        };
    }

    protected ActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
        {
            return NoContent();
        }

        return result.StatusType switch
        {
            StatusType.NotFound => NotFound(result.Errors),
            StatusType.ValidationError => BadRequest(result.Errors),
            StatusType.Conflict => Conflict(result.Errors),
            _ => BadRequest(result.Errors)
        };
    }
}
