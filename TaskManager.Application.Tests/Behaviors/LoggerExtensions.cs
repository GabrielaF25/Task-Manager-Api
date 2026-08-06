using Microsoft.Extensions.Logging;
using Moq;

namespace TaskManager.Application.Tests.Behaviors;

public static class LoggerExtensions
{
    public static void VerifyLog<T>(
    this Mock<ILogger<T>> logger,
    LogLevel level,
    string contains)
    {
        logger.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) =>
                    v.ToString()!.Contains(contains)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once());
    }
}
