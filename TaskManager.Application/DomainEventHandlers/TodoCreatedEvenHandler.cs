using MediatR;
using Microsoft.Extensions.Logging;
using TaskManager.Domain.Events;

namespace TaskManager.Application.DomainEventHandlers;

public class TodoCreatedEvenHandler : INotificationHandler<TodoCreatedEvent>
{
    private readonly ILogger<TodoCreatedEvenHandler> _logger;

    public TodoCreatedEvenHandler(ILogger<TodoCreatedEvenHandler> logger)
    {
        _logger = logger;
    }
    public Task Handle(TodoCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
       "User registered: {UserName}, Email: {Email}, OccurredOn: {OccurredOn}",
       notification.TodoItem.Title,
       notification.TodoItem.Description,
       notification.OccurredOn);

        return Task.CompletedTask;
    }
}
