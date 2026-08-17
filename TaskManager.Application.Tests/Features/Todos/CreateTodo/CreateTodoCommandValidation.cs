using FluentAssertions;
using TaskManager.Application.Features.Todos.CreateTodo;

namespace TaskManager.Application.Tests.Features.Todos.CreateTodo;

[TestFixture]
public class CreateTodoCommandValidationTests
{
    private CreateTodoCommandValidation _validator = null!;

    [SetUp]
    public void SetUp()
    {
        _validator = new CreateTodoCommandValidation();
    }

    [Test]
    public void Validate_Should_Return_Error_When_Title_Is_Empty()
    {
        // Arrange
        var command = new CreateTodoCommand(new CreateTodoRequest
        {
            Title = string.Empty,
            Description = "Description",
            ProjectId = Guid.NewGuid(),
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "TodoRequest.Title");
    }

    [Test]
    public void Validate_Should_Return_Error_When_Title_Exceeds_Maximum_Length()
    {
        // Arrange
        var command = new CreateTodoCommand(new CreateTodoRequest
        {
            Title = new string('A', 101),
            Description = "Description",
            ProjectId = Guid.NewGuid()
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "TodoRequest.Title");
    }

    [Test]
    public void Validate_Should_Return_Valid_When_Title_Is_Valid()
    {
        // Arrange
        var command = new CreateTodoCommand(new CreateTodoRequest
        {
            Title = "Learn NUnit",
            Description = "Write unit tests",
            ProjectId = Guid.NewGuid()
        });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}