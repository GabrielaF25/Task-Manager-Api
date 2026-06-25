using MediatR;
using TaskManager.Application.Common.ResultPattern;

namespace TaskManager.Application.Features.Projects.DeleteProject;

public record DeleteProjectCommand(int Id) : IRequest<Result>;

