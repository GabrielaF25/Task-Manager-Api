using MediatR;
using TaskManager.Application.Common.ResultPattern;

namespace TaskManager.Application.Features.Projects.DeleteProject;

public record DeleteProjectCommand(Guid Id) : IRequest<Result>;

