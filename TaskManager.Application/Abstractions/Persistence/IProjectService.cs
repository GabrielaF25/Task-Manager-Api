using TaskManager.Application.Common.Pagination;
using TaskManager.Application.Common.ResultPattern;
using TaskManager.Application.Features.Projects.Dto;
using TaskManager.Application.Features.Projects.Queries;

namespace TaskManager.Application.Abstractions.Persistence;

public interface IProjectService
{
    Task<Result<ProjectDto>> CreateProjectAsync(CreateProjectRequest project, CancellationToken ct);
    Task<Result<ProjectDto>> GetProjectDetailsByIdAsync(int id, CancellationToken ct);
    Task<Result<PaginationResult<ProjectDto>>> GetProjectsAsync(QueryParamProject queryParam, PaginationParam pagination,CancellationToken ct);
    Task<Result> RemoveAsync(int id, CancellationToken ct);
}