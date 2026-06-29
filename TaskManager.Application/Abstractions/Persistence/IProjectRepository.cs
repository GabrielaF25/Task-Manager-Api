using TaskManager.Application.Common.Pagination;
using TaskManager.Application.Features.Projects.GetProjects;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Abstractions.Persistence;

public interface IProjectRepository
{
    Task<Project> AddAsync(Project project, CancellationToken ct);
    Task<Project?> GetProjectByIdAsync(int id, CancellationToken ct);
    Task<Project?> GetProjectDetailsByIdAsync(int id, CancellationToken ct);
    Task<PaginationResult<Project>> GetProjectsAsync(QueryParamProject queryParam,int id, PaginationParam pagination,CancellationToken ct);
    void Remove(Project project);
}