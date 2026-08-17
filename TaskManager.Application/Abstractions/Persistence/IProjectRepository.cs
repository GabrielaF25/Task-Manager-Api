using TaskManager.Application.Common.Pagination;
using TaskManager.Application.Features.Projects.GetProjects;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Abstractions.Persistence;

public interface IProjectRepository
{
    Task<Project> AddAsync(Project project, CancellationToken ct);
    Task<Project?> GetProjectByIdAsync(Guid id, CancellationToken ct);
    Task<Project?> GetProjectDetailsByIdAsync(Guid id, CancellationToken ct);
    Task<PaginationResult<Project>> GetProjectsAsync(QueryParamProject queryParam, Guid id, PaginationParam pagination,CancellationToken ct);
    void Remove(Project project);
}