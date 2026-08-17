using FluentAssertions;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Events;

namespace TaskManager.Domain.Tests.Entities;

[TestFixture]
public class ProjectTests
{
    [Test]
    public void Create_Should_Set_Project_Properties()
    {
        // Arrange
        var name = "Task Manager";
        var description = "My test project";
        var ownerId = Guid.NewGuid();

        // Act
        var project = Project.Create(name, description, ownerId);

        // Assert
        project.Name.Should().Be(name);
        project.Description.Should().Be(description);
        project.OwnerId.Should().Be(ownerId);
    }

    [Test]
    public void Create_Should_Allow_Null_Description()
    {
        // Arrange
        var name = "Task Manager";
        var ownerId = Guid.NewGuid();

        // Act
        var project = Project.Create(name, null, ownerId);

        // Assert
        project.Description.Should().BeNull();
    }

    [Test]
    public void Create_Should_Initialize_TodoItems_As_Empty_Collection()
    {
        // Arrange
        var name = "Task Manager";
        var ownerId = Guid.NewGuid();

        // Act
        var project = Project.Create(name, null, ownerId);

        // Assert
        project.TodoItems.Should().NotBeNull();
        project.TodoItems.Should().BeEmpty();
    }

    [Test]
    public void Create_Should_Add_ProjectCreatedEvent()
    {
        // Arrange
        var name = "Task Manager";
        var ownerId = Guid.NewGuid();

        // Act
        var project = Project.Create(name, null, ownerId);

        // Assert
        project.DomainEvents.Should().ContainSingle();

        var domainEvent = project.DomainEvents.First();

        domainEvent.Should().BeOfType<ProjectCreatedEvent>();
    }

    [Test]
    public void Create_Should_Add_ProjectCreatedEvent_With_Created_Project()
    {
        // Arrange
        var name = "Task Manager";
        var ownerId = Guid.NewGuid();

        // Act
        var project = Project.Create(name, null, ownerId);

        // Assert
        var domainEvent = project.DomainEvents
            .OfType<ProjectCreatedEvent>()
            .Single();

        domainEvent.Project.Should().Be(project);
    }
}