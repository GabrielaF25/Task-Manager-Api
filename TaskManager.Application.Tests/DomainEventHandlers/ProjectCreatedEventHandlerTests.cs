using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using TaskManager.Application.DomainEventHandlers;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Events;

namespace TaskManager.Application.Tests.DomainEventHandlers;

[TestFixture]
public class ProjectCreatedEventHandlerTests
{
    private Mock<ILogger<ProjectCreatedEventHandler>> _loggerMock = null!;
    private ProjectCreatedEventHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<ProjectCreatedEventHandler>>();
        _handler = new ProjectCreatedEventHandler(_loggerMock.Object);
    }

    [Test]
    public async Task Handle_Should_Log_Information_When_Project_Is_Created()
    {
        // Arrange
        var project = Project.Create(
            "Task Manager",
            "Clean Architecture project",
            1);

        var domainEvent = new ProjectCreatedEvent(project);

        // Act
        await _handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) =>
                    state.ToString()!.Contains("User registered") &&
                    state.ToString()!.Contains(project.Name) &&
                    state.ToString()!.Contains(project.Description)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}