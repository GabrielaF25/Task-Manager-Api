using TaskManager.Domain.Common;
using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Events;

public record TodoCreatedEvent(TodoItem TodoItem) : IDomainEvent
{
    public DateTimeOffset OccurredOn => DateTimeOffset.UtcNow;
}
