using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Moq;
using TaskManager.Application.Behaviors;
using TaskManager.Application.Common.ResultPattern;

namespace TaskManager.Application.Tests.Behaviors;

[TestFixture]
public class ValidationBehaviorTests
{
    [Test]
    public async Task Handle_Should_Call_Next_When_No_Validators_Exist()
    {
        var validators = Enumerable.Empty<IValidator<TestRequest>>();
        var behavior = new ValidationBehavior<TestRequest, Result>(validators);

        var request = new TestRequest();
        var called = 0;

        RequestHandlerDelegate<Result> next = ct =>
        {
            called++;
            return Task.FromResult(Result.Success());
        };

        var result = await behavior.Handle(request, next, CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(called, Is.EqualTo(1));
    }

    [Test]
    public async Task Handle_Should_Call_Next_When_Validation_Passes()
    {
        var validatorMock = new Mock<IValidator<TestRequest>>();

        validatorMock
            .Setup(x => x.ValidateAsync(
                It.IsAny<ValidationContext<TestRequest>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var behavior = new ValidationBehavior<TestRequest, Result>(
            new[] { validatorMock.Object });

        var request = new TestRequest();
        var called = 0;

        RequestHandlerDelegate<Result> next = ct =>
        {
            called++;
            return Task.FromResult(Result.Success());
        };

        var result = await behavior.Handle(request, next, CancellationToken.None);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(called, Is.EqualTo(1));
    }

    [Test]
    public async Task Handle_Should_Return_ValidationError_When_Validation_Fails()
    {
        var validatorMock = new Mock<IValidator<TestRequest>>();

        validatorMock
            .Setup(x => x.ValidateAsync(
                It.IsAny<ValidationContext<TestRequest>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[]
            {
                new ValidationFailure("Name", "Name is required.")
            }));

        var behavior = new ValidationBehavior<TestRequest, Result>(
            new[] { validatorMock.Object });

        var request = new TestRequest();
        var called = 0;

        RequestHandlerDelegate<Result> next = ct =>
        {
            called++;
            return Task.FromResult(Result.Success());
        };

        var result = await behavior.Handle(request, next, CancellationToken.None);

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.StatusType, Is.EqualTo(StatusType.ValidationError));
        Assert.That(result.Errors, Does.Contain("Name is required."));
        Assert.That(called, Is.EqualTo(0));
    }

    [Test]
    public async Task Handle_Should_Return_Generic_ValidationError_When_Generic_Result_Fails()
    {
        var validatorMock = new Mock<IValidator<TestRequest>>();

        validatorMock
            .Setup(x => x.ValidateAsync(
                It.IsAny<ValidationContext<TestRequest>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[]
            {
                new ValidationFailure("Name", "Name is required.")
            }));

        var behavior = new ValidationBehavior<TestRequest, Result<TestResponse>>(
            new[] { validatorMock.Object });

        var request = new TestRequest();

        RequestHandlerDelegate<Result<TestResponse>> next = ct =>
            Task.FromResult(Result<TestResponse>.Success(new TestResponse()));

        var result = await behavior.Handle(request, next, CancellationToken.None);

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.StatusType, Is.EqualTo(StatusType.ValidationError));
        Assert.That(result.Errors, Does.Contain("Name is required."));
    }

    public sealed class TestRequest : IRequest<Result>
    {
    }

    public sealed class TestResponse
    {
        public string Name { get; set; } = string.Empty;
    }
}