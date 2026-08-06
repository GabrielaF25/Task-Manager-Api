using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManager.Application.Behaviors;
using TaskManager.Application.Common.ResultPattern;

namespace TaskManager.Application.Tests.Behaviors;

[TestFixture]
public class PerformanceBehaviorTests
{
    private Mock<ILogger<PerformanceBehavior<TestRequest, Result>>> _loggerMock = null!;
    private PerformanceBehavior<TestRequest, Result> _behavior = null!;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<PerformanceBehavior<TestRequest, Result>>>();
        _behavior = new PerformanceBehavior<TestRequest, Result>(_loggerMock.Object);
    }

    [Test]
    public async Task Handle_Should_Call_Next_Once()
    {
        var request = new TestRequest();
        var called = 0;

        RequestHandlerDelegate<Result> next = ct =>
        {
            called++;
            return Task.FromResult(Result.Success());
        };

        await _behavior.Handle(request, next, CancellationToken.None);

        Assert.That(called, Is.EqualTo(1));
    }

    [Test]
    public async Task Handle_Should_Return_Response_From_Next()
    {
        var request = new TestRequest();
        var expectedResponse = Result.Success();

        RequestHandlerDelegate<Result> next = ct =>
            Task.FromResult(expectedResponse);

        var result = await _behavior.Handle(request, next, CancellationToken.None);

        Assert.That(result, Is.SameAs(expectedResponse));
    }

    [Test]
    public async Task Handle_Should_Log_Execution_Time()
    {
        var request = new TestRequest();

        RequestHandlerDelegate<Result> next = ct =>
            Task.FromResult(Result.Success());

        await _behavior.Handle(request, next, CancellationToken.None);

        _loggerMock.VerifyLog(LogLevel.Information, "executed in");
        _loggerMock.VerifyLog(LogLevel.Information, nameof(TestRequest));
    }

    public sealed class TestRequest : IRequest<Result>
    {
    }
}