using MediatR;
using Microsoft.Extensions.Logging;
using TaskManager.Domain.Events;

namespace TaskManager.Application.DomainEventHandlers;

public class ProjectCreatedEventHandler : INotificationHandler<ProjectCreatedEvent>
{
    private readonly ILogger<ProjectCreatedEventHandler> _logger;

    public ProjectCreatedEventHandler(ILogger<ProjectCreatedEventHandler> logger)
    {
        _logger = logger;
    }
    public Task Handle(ProjectCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
         "User registered: {UserName}, Email: {Email}, OccurredOn: {OccurredOn}",
         notification.Project.Name,
         notification.Project.Description,
         notification.OccurredOn);

        return Task.CompletedTask;
    }
}
