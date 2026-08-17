using AutoMapper;
using FluentAssertions;
using Moq;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Abstractions.Services;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Projects.Dto;
using TaskManager.Application.Features.Projects.GetProjectDetails;
using TaskManager.Application.Features.Projects.Queries.GetProjectDetails;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Tests.Features.Projects.GetProjectDetails;

[TestFixture]
public class GetProjectDetailsQueryHandlerTests
{
    private Mock<IProjectRepository> _projectRepositoryMock = null!;
    private Mock<IMapper> _mapperMock = null!;
    private Mock<ICurrentUserService> _currentUserServiceMock = null!;
    private GetProjectDetailsQueryHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _mapperMock = new Mock<IMapper>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();

        _handler = new GetProjectDetailsQueryHandler(
            _projectRepositoryMock.Object,
            _mapperMock.Object,
            _currentUserServiceMock.Object);
    }

    [Test]
    public async Task Handle_Should_Return_NotFound_When_Project_Does_Not_Exist()
    {
        // Arrange
        var query = new GetProjectDetailsQuery(Guid.NewGuid());

        _projectRepositoryMock
            .Setup(x => x.GetProjectDetailsByIdAsync(query.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusType.Should().Be(StatusType.NotFound);
        result.Errors.Should().Contain("Project was not found.");

        _mapperMock.Verify(
            x => x.Map<ProjectDto>(It.IsAny<Project>()),
            Times.Never);
    }

    [Test]
    public async Task Handle_Should_Return_Forbidden_When_Current_User_Is_Not_Project_Owner()
    {
        // Arrange
        var query = new GetProjectDetailsQuery(Guid.NewGuid());

        var project = Project.Create(
            name: "Task Manager",
            description: "Test project",
            ownerId: Guid.NewGuid());

        _projectRepositoryMock
            .Setup(x => x.GetProjectDetailsByIdAsync(query.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        _currentUserServiceMock
            .Setup(x => x.GetCurrentUserId())
            .Returns(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusType.Should().Be(StatusType.Forbidden);
        result.Errors.Should().Contain("You are not authorize for viewing the project.");

        _mapperMock.Verify(
            x => x.Map<ProjectDto>(It.IsAny<Project>()),
            Times.Never);
    }

    [Test]
    public async Task Handle_Should_Return_ProjectDto_When_Current_User_Is_Project_Owner()
    {
        // Arrange
        var query = new GetProjectDetailsQuery(Guid.NewGuid());

        var project = Project.Create(
            name: "Task Manager",
            description: "Test project",
            ownerId: Guid.NewGuid());

        var projectDto = new ProjectDto
        {
            Name = "Task Manager",
            Description = "Test project"
        };

        _projectRepositoryMock
            .Setup(x => x.GetProjectDetailsByIdAsync(query.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        _currentUserServiceMock
            .Setup(x => x.GetCurrentUserId())
            .Returns(project.OwnerId);

        _mapperMock
            .Setup(x => x.Map<ProjectDto>(project))
            .Returns(projectDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(projectDto);

        _mapperMock.Verify(
            x => x.Map<ProjectDto>(project),
            Times.Once);
    }
}