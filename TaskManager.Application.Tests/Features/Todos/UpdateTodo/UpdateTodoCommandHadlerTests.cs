using AutoMapper;
using FluentAssertions;
using Moq;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Todos.Dtos;
using TaskManager.Application.Features.Todos.UpdateTodo;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Tests.Features.Todos.UpdateTodo;

[TestFixture]
public class UpdateTodoCommandHandlerTests
{
    private Mock<ITodoRepository> _todoRepositoryMock = null!;
    private Mock<IMapper> _mapperMock = null!;
    private UpdateTodoCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _todoRepositoryMock = new Mock<ITodoRepository>();
        _mapperMock = new Mock<IMapper>();

        _handler = new UpdateTodoCommandHandler(
            _todoRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Test]
    public async Task Handle_Should_Return_NotFound_When_Todo_Does_Not_Exist()
    {
        // Arrange
        var command = new UpdateTodoCommand(1);

        _todoRepositoryMock
            .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TodoItem?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusType.Should().Be(StatusType.NotFound);
        result.Errors.Should().Contain("The todo item was not found.");

        _mapperMock.Verify(
            x => x.Map<TodoResponse>(It.IsAny<TodoItem>()),
            Times.Never);
    }

    [Test]
    public async Task Handle_Should_Complete_Todo_When_Todo_Exists()
    {
        // Arrange
        var command = new UpdateTodoCommand(1);

        var todo = TodoItem.Create(
            "Learn NUnit",
            "Write tests",
            1);

        var todoResponse = new TodoResponse
        {
            Title = "Learn NUnit",
            Description = "Write tests"
        };

        _todoRepositoryMock
            .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(todo);

        _mapperMock
            .Setup(x => x.Map<TodoResponse>(todo))
            .Returns(todoResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        todo.IsCompleted.Should().BeTrue();
        result.Data.Should().Be(todoResponse);

        _mapperMock.Verify(
            x => x.Map<TodoResponse>(todo),
            Times.Once);
    }
}