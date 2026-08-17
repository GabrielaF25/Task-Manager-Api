using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Abstractions.Services;
using TaskManager.Application.Common.Pagination;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Projects.Dto;
using TaskManager.Application.Features.Projects.GetProjects;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Tests.Features.Projects.GetProjects;

[TestFixture]
public class GetProjectsQueryHandlerTests
{
    private Mock<IProjectRepository> _projectRepositoryMock = null!;
    private Mock<IMapper> _mapperMock = null!;
    private Mock<IValidator<PaginationParam>> _paginationValidatorMock = null!;
    private Mock<ICurrentUserService> _currentUserServiceMock = null!;
    private GetProjectsQueryHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _mapperMock = new Mock<IMapper>();
        _paginationValidatorMock = new Mock<IValidator<PaginationParam>>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();

        _handler = new GetProjectsQueryHandler(
            _projectRepositoryMock.Object,
            _mapperMock.Object,
            _paginationValidatorMock.Object,
            _currentUserServiceMock.Object);
    }

    [Test]
    public async Task Handle_Should_Return_ValidationError_When_Pagination_Is_Invalid()
    {
        // Arrange
        var queryParam = new QueryParamProject
        {
            Search = "task",
            SortBy = "name",
            SortDirection = "asc"
        };

        var pagination = new PaginationParam
        {
            PageNumber = 0,
            PageSize = 10
        };

        var query = new GetProjectQuery(queryParam, pagination);

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

        _currentUserServiceMock.Verify(
            x => x.GetCurrentUserId(),
            Times.Never);

        _projectRepositoryMock.Verify(
            x => x.GetProjectsAsync(
                It.IsAny<QueryParamProject>(),
                It.IsAny<Guid>(),
                It.IsAny<PaginationParam>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task Handle_Should_Return_Paginated_Projects_When_Pagination_Is_Valid()
    {
        // Arrange
        var queryParam = new QueryParamProject
        {
            Search = "task",
            SortBy = "name",
            SortDirection = "asc"
        };

        var pagination = new PaginationParam
        {
            PageNumber = 1,
            PageSize = 10
        };

        var query = new GetProjectQuery(queryParam, pagination);
        var ownerId = Guid.NewGuid();

        var projects = new List<Project>
        {
            Project.Create("Task Manager", "First project", ownerId),
            Project.Create("Learning App", "Second project", ownerId)
        };

        var projectDtos = new List<ProjectDto>
        {
            new ProjectDto
            {
                Name = "Task Manager",
                Description = "First project"
            },
            new ProjectDto
            {
                Name = "Learning App",
                Description = "Second project"
            }
        };

        var paginatedProjects = new PaginationResult<Project>
        {
            PageNumber = 1,
            PageSize = 10,
            TotalCount = 2,
            TotalPages = 1,
            HasNextPage = false,
            HasPreviousPage = false,
            Items = projects
        };

        _paginationValidatorMock
            .Setup(x => x.ValidateAsync(pagination, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _currentUserServiceMock
            .Setup(x => x.GetCurrentUserId())
            .Returns(ownerId);

        _projectRepositoryMock
            .Setup(x => x.GetProjectsAsync(
                queryParam,
                ownerId,
                pagination,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedProjects);

        _mapperMock
            .Setup(x => x.Map<IEnumerable<ProjectDto>>(projects))
            .Returns(projectDtos);

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
        result.Data.Items.Should().BeEquivalentTo(projectDtos);

        _currentUserServiceMock.Verify(
            x => x.GetCurrentUserId(),
            Times.Once);

        _projectRepositoryMock.Verify(
            x => x.GetProjectsAsync(
                queryParam,
                ownerId,
                pagination,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<IEnumerable<ProjectDto>>(projects),
            Times.Once);
    }
}