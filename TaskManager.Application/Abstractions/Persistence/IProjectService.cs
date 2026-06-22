using TaskManager.Application.Common.Pagination;
using TaskManager.Application.Features.Projects.Dto;
using TaskManager.Application.Features.Projects.Queries;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Abstractions.Persistence;

public interface IProjectService
{
    Task<ProjectDto> CreateProjectAsync(CreateProjectRequest project, CancellationToken ct);
    Task<ProjectDto?> GetProjectDetailsByIdAsync(int id, CancellationToken ct);
    Task<PaginationResult<ProjectDto>> GetProjectsAsync(QueryParamProject queryParam, PaginationParam pagination,CancellationToken ct);
    Task<bool> RemoveAsync(int id, CancellationToken ct);
}