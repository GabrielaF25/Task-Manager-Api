using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Common.Pagination;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Todos.Dtos;
using TaskManager.Application.Features.Todos.GetTodos;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Tests.Features.Todos.GetTodos;

[TestFixture]
public class GetTodosQueryHandlerTests
{
    private Mock<ITodoRepository> _todoRepositoryMock = null!;
    private Mock<IMapper> _mapperMock = null!;
    private Mock<IValidator<PaginationParam>> _paginationValidatorMock = null!;
    private GetTodosQueryHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _todoRepositoryMock = new Mock<ITodoRepository>();
        _mapperMock = new Mock<IMapper>();
        _paginationValidatorMock = new Mock<IValidator<PaginationParam>>();

        _handler = new GetTodosQueryHandler(
            _todoRepositoryMock.Object,
            _mapperMock.Object,
            _paginationValidatorMock.Object);
    }

    [Test]
    public async Task Handle_Should_Return_ValidationError_When_Pagination_Is_Invalid()
    {
        // Arrange
        var queryParam = new QueryParamTodo
        {
            Search = "test",
            IsCompleted = false,
            SortBy = "title",
            SortDirection = "asc"
        };

        var pagination = new PaginationParam
        {
            PageNumber = 0,
            PageSize = 10
        };

        var query = new GetTodosQuery(queryParam, pagination);

        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("PageNumber", "Page number must be greater than 0.")
        });

        _paginationValidatorMock
            .Setup(x => x.ValidateAsync(pagination, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusType.Should().Be(StatusType.ValidationError);
        result.Errors.Should().Contain("Page number must be greater than 0.");

        _todoRepositoryMock.Verify(
            x => x.GetAllAsync(
                It.IsAny<QueryParamTodo>(),
                It.IsAny<PaginationParam>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task Handle_Should_Return_Paginated_Todos_When_Pagination_Is_Valid()
    {
        // Arrange
        var queryParam = new QueryParamTodo
        {
            Search = "test",
            IsCompleted = false,
            SortBy = "title",
            SortDirection = "asc"
        };

        var pagination = new PaginationParam
        {
            PageNumber = 1,
            PageSize = 10
        };

        var query = new GetTodosQuery(queryParam, pagination);

        var todos = new List<TodoItem>
        {
            TodoItem.Create("Learn NUnit", "Write tests", 1),
            TodoItem.Create("Learn Moq", "Mock dependencies", 1)
        };

        var todoResponses = new List<TodoResponse>
        {
            new TodoResponse
            {
                Title = "Learn NUnit",
                Description = "Write tests"
            },
            new TodoResponse
            {
                Title = "Learn Moq",
                Description = "Mock dependencies"
            }
        };

        var paginatedTodos = new PaginationResult<TodoItem>
        {
            PageNumber = 1,
            PageSize = 10,
            TotalCount = 2,
            TotalPages = 1,
            HasNextPage = false,
            HasPreviousPage = false,
            Items = todos
        };

        _paginationValidatorMock
            .Setup(x => x.ValidateAsync(pagination, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _todoRepositoryMock
            .Setup(x => x.GetAllAsync(
                queryParam,
                pagination,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedTodos);

        _mapperMock
            .Setup(x => x.Map<IEnumerable<TodoResponse>>(todos))
            .Returns(todoResponses);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();

        result.Data!.PageNumber.Should().Be(1);
        result.Data.PageSize.Should().Be(10);
        result.Data.TotalCount.Should().Be(2);
        result.Data.TotalPages.Should().Be(1);
        result.Data.HasNextPage.Should().BeFalse();
        result.Data.HasPreviousPage.Should().BeFalse();
        result.Data.Items.Should().BeEquivalentTo(todoResponses);

        _todoRepositoryMock.Verify(
            x => x.GetAllAsync(
                queryParam,
                pagination,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<IEnumerable<TodoResponse>>(todos),
            Times.Once);
    }
}