using FluentAssertions;
using Moq;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Abstractions.Services;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Projects.DeleteProject;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Tests.Features.Projects.DeleteProject;

[TestFixture]
public class DeleteProjectCommandHandlerTests
{
    private Mock<IProjectRepository> _projectRepositoryMock = null!;
    private Mock<ICurrentUserService> _currentUserServiceMock = null!;
    private DeleteProjectCommandHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();

        _handler = new DeleteProjectCommandHandler(
            _projectRepositoryMock.Object,
            _currentUserServiceMock.Object);
    }

    [Test]
    public async Task Handle_Should_Return_NotFound_When_Project_Does_Not_Exist()
    {
        // Arrange
        var command = new DeleteProjectCommand(Guid.NewGuid());

        _projectRepositoryMock
            .Setup(x => x.GetProjectByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusType.Should().Be(StatusType.NotFound);
        result.Errors.Should().Contain("The project was not found");

        _projectRepositoryMock.Verify(
            x => x.Remove(It.IsAny<Project>()),
            Times.Never);
    }

    [Test]
    public async Task Handle_Should_Return_Forbidden_When_Current_User_Is_Not_Project_Owner()
    {
        // Arrange
        var command = new DeleteProjectCommand(Guid.NewGuid());

        var project = Project.Create(
            "Task Manager",
            "Description",
            ownerId: Guid.NewGuid());

        _projectRepositoryMock
            .Setup(x => x.GetProjectByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        _currentUserServiceMock
            .Setup(x => x.GetCurrentUserId())
            .Returns(Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusType.Should().Be(StatusType.Forbidden);
        result.Errors.Should().Contain("You are not authorize to delete the project");

        _projectRepositoryMock.Verify(
            x => x.Remove(It.IsAny<Project>()),
            Times.Never);
    }

    [Test]
    public async Task Handle_Should_Delete_Project_When_Current_User_Is_Project_Owner()
    {
        // Arrange
        var command = new DeleteProjectCommand(Guid.NewGuid());
         
        var project = Project.Create(
            "Task Manager",
            "Description",
            ownerId: Guid.NewGuid());

        _projectRepositoryMock
            .Setup(x => x.GetProjectByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        _currentUserServiceMock
            .Setup(x => x.GetCurrentUserId())
            .Returns(project.OwnerId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _projectRepositoryMock.Verify(
            x => x.Remove(project),
            Times.Once);
    }
}