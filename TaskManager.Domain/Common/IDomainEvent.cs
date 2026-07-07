using MediatR;

namespace TaskManager.Domain.Common;

public interface IDomainEvent : INotification
{
    DateTimeOffset OccurredOn { get;}
}
