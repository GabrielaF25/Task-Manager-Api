using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using TaskManager.Application.DomainEventHandlers;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Events;

namespace TaskManager.Application.Tests.DomainEventHandlers;

[TestFixture]
public class TodoCreatedEventHandlerTests
{
    private Mock<ILogger<TodoCreatedEventHandler>> _loggerMock = null!;
    private TodoCreatedEventHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<TodoCreatedEventHandler>>();
        _handler = new TodoCreatedEventHandler(_loggerMock.Object);
    }

    [Test]
    public async Task Handle_Should_Log_Information_When_Todo_Is_Created()
    {
        // Arrange
        var todo = TodoItem.Create(
            "Implement tests",
            "Write unit tests for event handlers",
            Guid.NewGuid());

        var domainEvent = new TodoCreatedEvent(todo);

        // Act
        await _handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) =>
                    state.ToString()!.Contains("User registered") &&
                    state.ToString()!.Contains(todo.Title) &&
                    state.ToString()!.Contains(todo.Description)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}