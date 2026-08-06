using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using TaskManager.Application.DomainEventHandlers;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Events;

namespace TaskManager.Application.Tests.DomainEventHandlers;

[TestFixture]
public class UserRegisteredEventHandlerTests
{
    private Mock<ILogger<UserRegisteredEventHandler>> _loggerMock = null!;
    private UserRegisteredEventHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<UserRegisteredEventHandler>>();
        _handler = new UserRegisteredEventHandler(_loggerMock.Object);
    }

    [Test]
    public async Task Handle_Should_Log_Information_When_User_Is_Registered()
    {
        // Arrange
        var user = User.Register(
            "gabriel@test.com",
            "Gabriel");

        var domainEvent = new UserRegisteredEvent(user);

        // Act
        await _handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) =>
                    state.ToString()!.Contains("User registered") &&
                    state.ToString()!.Contains(user.UserName) &&
                    state.ToString()!.Contains(user.Email)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}