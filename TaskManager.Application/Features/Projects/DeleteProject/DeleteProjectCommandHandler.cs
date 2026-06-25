using MediatR;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Common.ResultPattern;

namespace TaskManager.Application.Features.Projects.DeleteProject;

public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand,Result>
{
    private readonly IProjectRepository _projectRepository;

    public DeleteProjectCommandHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<Result> Handle(DeleteProjectCommand deleteProject, CancellationToken ct)
    {
        var project = await _projectRepository.GetProjectByIdAsync(deleteProject.Id, ct);

        if (project == null)
        {
            var error = new List<string> { "The project was not found" };
            return Result.Failed(error, StatusType.NotFound);
        }

        _projectRepository.Remove(project);
        await _projectRepository.SaveChangesAsync(ct);

        return Result.Success();

    }
}
