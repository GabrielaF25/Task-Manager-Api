using MediatR;
using Microsoft.Extensions.Logging;
using TaskManager.Domain.Events;

namespace TaskManager.Application.DomainEventHandlers;

public class UserRegisteredEventHandler : INotificationHandler<UserRegisteredEvent>
{
    private readonly ILogger<UserRegisteredEventHandler> _logger;

    public UserRegisteredEventHandler(ILogger<UserRegisteredEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
          "User registered: {UserName}, Email: {Email}, OccurredOn: {OccurredOn}",
          notification.User.UserName,
          notification.User.Email,
          notification.OccurredOn);

        return Task.CompletedTask;
    }
}
