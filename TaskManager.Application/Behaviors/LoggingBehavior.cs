using MediatR;
using Microsoft.Extensions.Logging;
using System.Buffers;
using TaskManager.Application.Common.ResultPattern;

namespace TaskManager.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull where TResponse : Result
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        _logger.LogInformation("Handling {RequestName}", requestName);

        var response = await next(cancellationToken);

        if (response.IsSuccess)
        {
            _logger.LogInformation("Handled {RequestName} successfully", requestName);
        }
        else
        {
            _logger.LogWarning(
                "Handled {RequestName} with errors: {Errors}",
                requestName,
                response.Errors);
        }

        return response;
    }
}
