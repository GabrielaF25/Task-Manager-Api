using FluentAssertions;
using Moq;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Todos.DeleteTodo;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Tests.Features.Todos.DeleteTodo;

[TestFixture]
public class DeleteTodoCommandHandlerTests
{
    private Mock<ITodoRepository> _todoRepositoryMock = null!;
    private DeleteTodoCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _todoRepositoryMock = new Mock<ITodoRepository>();

        _handler = new DeleteTodoCommandHandler(_todoRepositoryMock.Object);
    }

    [Test]
    public async Task Handle_Should_Return_NotFound_When_Todo_Does_Not_Exist()
    {
        // Arrange
        var command = new DeleteTodoCommand(Guid.NewGuid());

        _todoRepositoryMock
            .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TodoItem?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusType.Should().Be(StatusType.NotFound);
        result.Errors.Should().Contain("The todo item was not found.");

        _todoRepositoryMock.Verify(
            x => x.Remove(It.IsAny<TodoItem>()),
            Times.Never);
    }

    [Test]
    public async Task Handle_Should_Delete_Todo_When_Todo_Exists()
    {
        // Arrange
        var command = new DeleteTodoCommand(Guid.NewGuid());

        var todo = TodoItem.Create(
            "Learn NUnit",
            "Write handler tests",
            Guid.NewGuid());

        _todoRepositoryMock
            .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(todo);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _todoRepositoryMock.Verify(
            x => x.Remove(todo),
            Times.Once);
    }
}