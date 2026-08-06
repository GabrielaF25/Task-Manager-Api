using AutoMapper;
using FluentAssertions;
using Moq;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Abstractions.Services;
using TaskManager.Application.Features.Projects.CreateProject;
using TaskManager.Application.Features.Projects.Dto;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Tests.Features.Projects.CreateProject;

[TestFixture]
public class CreateProjectCommandHandlerTests
{
    private Mock<IProjectRepository> _projectRepositoryMock = null!;
    private Mock<IMapper> _mapperMock = null!;
    private Mock<ICurrentUserService> _currentUserServiceMock = null!;
    private CreateProjectCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _mapperMock = new Mock<IMapper>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();

        _handler = new CreateProjectCommandHandler(
            _projectRepositoryMock.Object,
            _mapperMock.Object,
            _currentUserServiceMock.Object);
    }

    [Test]
    public async Task Handle_Should_Create_Project_With_Current_User_Id()
    {
        // Arrange
        var command = new CreateProjectCommand(new CreateProjectRequest
        {
            Name = "Task Manager",
            Description = "Test project"
        });

        _currentUserServiceMock
            .Setup(x => x.GetCurrentUserId())
            .Returns(10);

        _projectRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project project, CancellationToken _) => project);

        _mapperMock
            .Setup(x => x.Map<ProjectDto>(It.IsAny<Project>()))
            .Returns(new ProjectDto
            {
                Name = "Task Manager",
                Description = "Test project"
            });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _projectRepositoryMock.Verify(x =>
            x.AddAsync(
                It.Is<Project>(p =>
                    p.Name == "Task Manager" &&
                    p.Description == "Test project" &&
                    p.OwnerId == 10),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task Handle_Should_Return_ProjectDto_When_Project_Is_Created()
    {
        // Arrange
        var command = new CreateProjectCommand(new CreateProjectRequest
        {
            Name = "Task Manager",
            Description = "Test project"
        });

        var project = Project.Create("Task Manager", "Test project", 10);

        var projectDto = new ProjectDto
        {
            Name = "Task Manager",
            Description = "Test project"
        };

        _currentUserServiceMock
            .Setup(x => x.GetCurrentUserId())
            .Returns(10);

        _projectRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        _mapperMock
            .Setup(x => x.Map<ProjectDto>(project))
            .Returns(projectDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(projectDto);
    }
}