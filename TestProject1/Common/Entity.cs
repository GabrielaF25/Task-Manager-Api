using FluentAssertions;
using TaskManager.Domain.Common;

namespace TaskManager.Domain.Tests.Common;

[TestFixture]
public class EntityTests
{
    [Test]
    public void AddDomainEvent_Should_Add_Event()
    {
        // Arrange
        var entity = new TestEntity();
        var domainEvent = new TestDomainEvent();

        // Act
        entity.AddTestDomainEvent(domainEvent);

        // Assert
        entity.DomainEvents.Should().ContainSingle();
        entity.DomainEvents.Should().Contain(domainEvent);
    }

    [Test]
    public void ClearDomainEvent_Should_Remove_All_Events()
    {
        // Arrange
        var entity = new TestEntity();

        entity.AddTestDomainEvent(new TestDomainEvent());
        entity.AddTestDomainEvent(new TestDomainEvent());

        // Act
        entity.ClearDomainEvent();

        // Assert
        entity.DomainEvents.Should().BeEmpty();
    }

    private class TestEntity : Entity
    {
        public void AddTestDomainEvent(IDomainEvent domainEvent)
        {
            AddDomainEvent(domainEvent);
        }
    }

    private class TestDomainEvent : IDomainEvent
    {
        public DateTimeOffset OccurredOn => DateTimeOffset.UtcNow;
    }
}