using MediatR;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Projects.Dto;

namespace TaskManager.Application.Features.Projects.Queries.GetProjectDetails;

public record GetProjectDetailsQuery(int Id) : IRequest<Result<ProjectDto>>;// IRequest TResponse