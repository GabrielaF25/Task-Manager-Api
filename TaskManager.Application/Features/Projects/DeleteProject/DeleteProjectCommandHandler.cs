using MediatR;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Abstractions.Services;
using TaskManager.Application.Common.ResultPattern;

namespace TaskManager.Application.Features.Projects.DeleteProject;

public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand,Result>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ICurrentUserService _currentUserService;

    public DeleteProjectCommandHandler(
        IProjectRepository projectRepository,
        ICurrentUserService currentUserService)
    {
        _projectRepository = projectRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(DeleteProjectCommand deleteProject, CancellationToken ct)
    {
        var project = await _projectRepository.GetProjectByIdAsync(deleteProject.Id, ct);

        if (project == null)
        {
            var error = new List<string> { "The project was not found" };
            return Result.Failed(error, StatusType.NotFound);
        }

        var ownerId = _currentUserService.GetCurrentUserId();
        if (project.OwnerId != ownerId)
        {
            var error = new List<string> { "You are not authorize to delete the project" };
            return Result.Failed(error, StatusType.Forbidden);
        }

        _projectRepository.Remove(project);

        return Result.Success();

    }
}
