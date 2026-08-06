using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using TaskManager.Application.Behaviors;
using TaskManager.Application.Common.ResultPattern;

namespace TaskManager.Application.Tests.Behaviors;

[TestFixture]
public class LoggingBehaviorTests
{
    private Mock<ILogger<LoggingBehavior<TestRequest, Result>>> _loggerMock = null!;
    private LoggingBehavior<TestRequest, Result> _behavior = null!;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<LoggingBehavior<TestRequest, Result>>>();

        _behavior = new LoggingBehavior<TestRequest, Result>(
            _loggerMock.Object);
    }

    [Test]
    public async Task Handle_Should_Log_Information_When_Request_Is_Successful()
    {
        // Arrange
        var request = new TestRequest();

        RequestHandlerDelegate<Result> next = ct =>
            Task.FromResult(Result.Success());

        // Act
        var result = await _behavior.Handle(
            request,
            next,
            CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.True);

        _loggerMock.VerifyLog(LogLevel.Information, "Handling");
        _loggerMock.VerifyLog(LogLevel.Information, "successfully");
    }

    [Test]
    public async Task Handle_Should_Log_Warning_When_Request_Fails()
    {
        // Arrange
        var request = new TestRequest();

        RequestHandlerDelegate<Result> next = ct =>
            Task.FromResult(Result.Failed(
                new List<string> { "Error" },
                StatusType.ValidationError));

        // Act
        var result = await _behavior.Handle(
            request,
            next,
            CancellationToken.None);

        // Assert
        Assert.That(result.IsSuccess, Is.False);

        _loggerMock.VerifyLog(LogLevel.Information, "Handling");
        _loggerMock.VerifyLog(LogLevel.Warning, "errors");
    }

    [Test]
    public async Task Handle_Should_Invoke_Next_Once()
    {
        // Arrange
        var request = new TestRequest();

        var called = 0;

        RequestHandlerDelegate<Result> next = ct =>
        {
            called++;
            return Task.FromResult(Result.Success());
        };

        // Act
        await _behavior.Handle(
            request,
            next,
            CancellationToken.None);

        // Assert
        Assert.That(called, Is.EqualTo(1));
    }

    public sealed class TestRequest : IRequest<Result>
    {
    }
}