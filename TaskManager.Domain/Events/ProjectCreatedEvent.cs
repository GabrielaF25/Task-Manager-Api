using TaskManager.Domain.Common;
using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Events;

public record ProjectCreatedEvent(Project Project) : IDomainEvent
{
    public DateTimeOffset OccurredOn => DateTimeOffset.UtcNow;
}
