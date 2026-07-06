namespace TaskManager.Domain.Common;

public interface IDomainEvent
{
    DateTimeOffset OccurrendOn { get;}
}
