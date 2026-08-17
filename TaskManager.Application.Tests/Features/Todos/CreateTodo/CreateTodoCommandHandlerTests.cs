using AutoMapper;
using FluentAssertions;
using Moq;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Features.Todos.CreateTodo;
using TaskManager.Application.Features.Todos.Dtos;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Tests.Features.Todos.CreateTodo;

[TestFixture]
public class CreateTodoCommandHandlerTests
{
    private Mock<ITodoRepository> _todoRepositoryMock = null!;
    private Mock<IMapper> _mapperMock = null!;
    private CreateTodoCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _todoRepositoryMock = new Mock<ITodoRepository>();
        _mapperMock = new Mock<IMapper>();

        _handler = new CreateTodoCommandHandler(
            _todoRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Test]
    public async Task Handle_Should_Create_Todo()
    {
        // Arrange
        var command = new CreateTodoCommand(new CreateTodoRequest
        {
            Title = "Learn tests",
            Description = "Write NUnit tests",
            ProjectId = Guid.NewGuid()
        });

        _todoRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<TodoItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TodoItem todo, CancellationToken _) => todo);

        _mapperMock
            .Setup(x => x.Map<TodoResponse>(It.IsAny<TodoItem>()))
            .Returns(new TodoResponse
            {
                Title = "Learn tests",
                Description = "Write NUnit tests"
            });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _todoRepositoryMock.Verify(x =>
            x.AddAsync(
                It.Is<TodoItem>(t =>
                    t.Title == "Learn tests" &&
                    t.Description == "Write NUnit tests" &&
                    t.ProjectId == command.TodoRequest.ProjectId),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task Handle_Should_Return_TodoResponse_When_Todo_Is_Created()
    {
        // Arrange
        var command = new CreateTodoCommand(new CreateTodoRequest
        {
            Title = "Learn tests",
            Description = "Write NUnit tests",
            ProjectId = Guid.NewGuid()
        });

        var todo = TodoItem.Create("Learn tests", "Write NUnit tests", Guid.NewGuid());

        var todoResponse = new TodoResponse
        {
            Title = "Learn tests",
            Description = "Write NUnit tests"
        };

        _todoRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<TodoItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(todo);

        _mapperMock
            .Setup(x => x.Map<TodoResponse>(todo))
            .Returns(todoResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(todoResponse);

        _mapperMock.Verify(
            x => x.Map<TodoResponse>(todo),
            Times.Once);
    }
}