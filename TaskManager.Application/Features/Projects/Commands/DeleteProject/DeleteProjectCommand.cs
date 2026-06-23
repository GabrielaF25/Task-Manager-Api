using MediatR;
using TaskManager.Application.Common.ResultPattern;

namespace TaskManager.Application.Features.Projects.Commands.DeleteProject;

public record DeleteProjectCommand(int Id) : IRequest<Result>;

