using FluentAssertions;
using TaskManager.Application.Features.Projects.CreateProject;
using TaskManager.Application.Features.Projects.Dto;

namespace TaskManager.Application.Tests.Features.Projects.CreateProjects;

[TestFixture]
public class CreateProjectCommandValidationTests
{
    private CreateProjectCommandValidation _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new CreateProjectCommandValidation();
    }

    [Test]
    public void Validate_Should_Return_Error_When_Name_Is_Empty()
    {
        // Arrange
        var command = new CreateProjectCommand(new CreateProjectRequest
        {
            Name = string.Empty,
            Description = "Description"
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "Project.Name");
    }

    [Test]
    public void Validate_Should_Return_Error_When_Name_Exceeds_Maximum_Length()
    {
        // Arrange
        var command = new CreateProjectCommand(new CreateProjectRequest
        {
            Name = new string('A', 101),
            Description = "Description"
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "Project.Name");
    }

    [Test]
    public void Validate_Should_Return_Error_When_Description_Exceeds_Maximum_Length()
    {
        // Arrange
        var command = new CreateProjectCommand(new CreateProjectRequest
        {
            Name = "Project",
            Description = new string('A', 501)
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "Project.Description");
    }

    [Test]
    public void Validate_Should_Return_Valid_When_Description_Is_Null()
    {
        // Arrange
        var command = new CreateProjectCommand(new CreateProjectRequest
        {
            Name = "Project",
            Description = null
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void Validate_Should_Return_Valid_When_Request_Is_Valid()
    {
        // Arrange
        var command = new CreateProjectCommand(new CreateProjectRequest
        {
            Name = "Project",
            Description = "Project description"
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}