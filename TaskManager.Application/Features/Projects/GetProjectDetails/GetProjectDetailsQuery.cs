using MediatR;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Projects.Dto;

namespace TaskManager.Application.Features.Projects.GetProjectDetails;

public record GetProjectDetailsQuery(Guid Id) : IRequest<Result<ProjectDto>>;// IRequest TResponse