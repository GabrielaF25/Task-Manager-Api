using AutoMapper;
using FluentAssertions;
using Moq;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Todos.Dtos;
using TaskManager.Application.Features.Todos.GetTodo;
using TaskManager.Application.Features.Todos.Queries.GetTodo;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Tests.Features.Todos.QueryTodo;

[TestFixture]
public class GetTodoQueryHandlerTests
{
    private Mock<ITodoRepository> _todoRepositoryMock = null!;
    private Mock<IMapper> _mapperMock = null!;
    private GetTodoQueryHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _todoRepositoryMock = new Mock<ITodoRepository>();
        _mapperMock = new Mock<IMapper>();

        _handler = new GetTodoQueryHandler(
            _todoRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Test]
    public async Task Handle_Should_Return_NotFound_When_Todo_Does_Not_Exist()
    {
        // Arrange
        var query = new GetTodoQuery(Guid.NewGuid());

        _todoRepositoryMock
            .Setup(x => x.GetByIdAsync(query.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TodoItem?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusType.Should().Be(StatusType.NotFound);
        result.Errors.Should().Contain("The todo item was not found.");

        _mapperMock.Verify(
            x => x.Map<TodoResponse>(It.IsAny<TodoItem>()),
            Times.Never);
    }

    [Test]
    public async Task Handle_Should_Return_TodoResponse_When_Todo_Exists()
    {
        // Arrange
        var query = new GetTodoQuery(Guid.NewGuid());

        var todo = TodoItem.Create(
            "Learn NUnit",
            "Write unit tests",
            Guid.NewGuid());

        var todoResponse = new TodoResponse
        {
            Title = "Learn NUnit",
            Description = "Write unit tests"
        };

        _todoRepositoryMock
            .Setup(x => x.GetByIdAsync(query.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(todo);

        _mapperMock
            .Setup(x => x.Map<TodoResponse>(todo))
            .Returns(todoResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(todoResponse);

        _mapperMock.Verify(
            x => x.Map<TodoResponse>(todo),
            Times.Once);
    }
}