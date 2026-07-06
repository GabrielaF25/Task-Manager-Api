using TaskManager.Domain.Common;

namespace TaskManager.Domain.Events;

public sealed record UserRegisteredEvent(int UserId) : IDomainEvent
{
    public DateTimeOffset OccurrendOn => DateTimeOffset.UtcNow;
}
