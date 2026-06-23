using MediatR;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Projects.Dto;

namespace TaskManager.Application.Features.Projects.Commands.CreateProject;

public record CreateProjectCommand(CreateProjectRequest Project) : IRequest<Result<ProjectDto>>;
