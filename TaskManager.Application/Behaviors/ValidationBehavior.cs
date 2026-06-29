using FluentValidation;
using MediatR;
using TaskManager.Application.Common.ResultPattern;

namespace TaskManager.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next(cancellationToken);
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResult = await Task.WhenAll( _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var errors = validationResult
            .SelectMany(v => v.Errors)
            .Where(e => e is not null)
            .Select(e => e.ErrorMessage)
            .ToList();

        if(errors.Count != 0)
        {
            return CreateValidationResult<TResponse>(errors);
        }

        return await next(cancellationToken);
    }

    private static TResponse CreateValidationResult<TResponseResult>(
        List<string> errors) where TResponseResult : Result
    {
        if (typeof(TResponseResult).IsGenericType)
        {
            var resultType = typeof(TResponseResult).GetGenericArguments()[0];

            var failedMethod = typeof(Result<>).MakeGenericType(resultType).GetMethod(nameof(Result<object>.Failed),
                new[] {typeof(List<string>), typeof(StatusType)} );

            return (TResponse)(failedMethod!.Invoke(null, new object[] { errors, StatusType.ValidationError })!);
        }
        return (TResponse)(object)Result.Failed(errors, StatusType.ValidationError);
    }
}
