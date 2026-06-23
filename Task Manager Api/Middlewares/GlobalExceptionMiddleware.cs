using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Task_Manager_Api.Middlewares;

public class GlobalExceptionMiddleware: IExceptionHandler
{
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    public GlobalExceptionMiddleware(ILogger<GlobalExceptionMiddleware> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError($"An error occured while processing your request: {exception.Message}");

        var problemDetails = new ProblemDetails
        {
            Detail = exception.Message,
            Title = exception.GetType().Name,
            Instance = httpContext.Request.Path
        };

        switch (exception)
        {

            case UnauthorizedAccessException:
                problemDetails.Status = (int)HttpStatusCode.Unauthorized;
                break;

            default:
                problemDetails.Status = (int)HttpStatusCode.InternalServerError;
                problemDetails.Title = "Internal Server Error";
                break;
        }

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;

    }
}
