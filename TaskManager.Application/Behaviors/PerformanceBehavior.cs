using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using TaskManager.Application.Common.ResultPattern;

namespace TaskManager.Application.Behaviors;

public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull where TResponse : Result
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>>  _logger;

    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
       var stopwatch = Stopwatch.StartNew();

        var response = await next();

        stopwatch.Stop();

        _logger.LogInformation(
            "{RequestName} executed in {ElapsedMilliseconds} ms",
            typeof(TRequest).Name,
            stopwatch.ElapsedMilliseconds);

        return response;
    }
}
