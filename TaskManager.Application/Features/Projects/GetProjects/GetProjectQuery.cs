using MediatR;
using TaskManager.Application.Common.Pagination;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Projects.Dto;

namespace TaskManager.Application.Features.Projects.GetProjects;

public record  GetProjectQuery(QueryParamProject QueryParam, PaginationParam Pagination) : IRequest<Result<PaginationResult<ProjectDto>>>;