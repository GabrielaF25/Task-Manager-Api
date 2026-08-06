using FluentAssertions;
using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Tests.Entities;

[TestFixture]
public class TodoItemTests
{
    [Test]
    public void Create_Should_Set_Properties()
    {
        // Arrange
        var title = "Implement login";
        var description = "Use JWT";
        var projectId = 1;

        // Act
        var todoItem = TodoItem.Create(title, description, projectId);

        // Assert
        todoItem.Title.Should().Be(title);
        todoItem.Description.Should().Be(description);
        todoItem.ProjectId.Should().Be(projectId);
        todoItem.IsCompleted.Should().BeFalse();
    }

    [Test]
    public void Create_Should_Allow_Null_Description()
    {
        // Arrange
        var title = "Implement login";
        var projectId = 1;

        // Act
        var todoItem = TodoItem.Create(title, null, projectId);

        // Assert
        todoItem.Description.Should().BeNull();
    }

    [Test]
    public void Create_Should_Set_CreatedAt()
    {
        // Arrange
        var before = DateTimeOffset.UtcNow;

        // Act
        var todoItem = TodoItem.Create("Task", null, 1);

        var after = DateTimeOffset.UtcNow;

        // Assert
        todoItem.CreatedAt.Should().BeOnOrAfter(before);
        todoItem.CreatedAt.Should().BeOnOrBefore(after);
    }

    [Test]
    public void Complete_Should_Set_IsCompleted_To_True()
    {
        // Arrange
        var todoItem = TodoItem.Create("Task", null, 1);

        // Act
        todoItem.Complete();

        // Assert
        todoItem.IsCompleted.Should().BeTrue();
    }

    [Test]
    public void Complete_Should_Remain_Completed_When_Called_Multiple_Times()
    {
        // Arrange
        var todoItem = TodoItem.Create("Task", null, 1);

        // Act
        todoItem.Complete();
        todoItem.Complete();

        // Assert
        todoItem.IsCompleted.Should().BeTrue();
    }
}