using MediatR;
using Microsoft.Extensions.Logging;
using TaskManager.Domain.Events;

namespace TaskManager.Application.DomainEventHandlers;

public class TodoCreatedEventHandler : INotificationHandler<TodoCreatedEvent>
{
    private readonly ILogger<TodoCreatedEventHandler> _logger;

    public TodoCreatedEventHandler(ILogger<TodoCreatedEventHandler> logger)
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
