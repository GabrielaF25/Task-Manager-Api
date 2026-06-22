using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Common.Pagination;
using TaskManager.Application.Features.Projects.Queries;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.DbContexts;
using TaskManager.Infrastructure.ExtensionMethods;

namespace TaskManager.Infrastructure.Repository;

public class ProjectRepository : IProjectRepository
{
    private readonly TaskManagerDbContext _dbContext;

    public ProjectRepository(TaskManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Project> AddAsync(Project project, CancellationToken ct)
    {
        await _dbContext.AddAsync(project, ct);
        return project;
    }

    public async Task<PaginationResult<Project>> GetProjectsAsync(QueryParamProject queryParam,PaginationParam pagination,CancellationToken ct)
    {
        var query =  _dbContext.Projects
            .ApplyProjectFilters(queryParam)
            .ApplyProjectSorting(queryParam)
            .AsNoTracking();

        var totalCount = await query.CountAsync(ct);

        var projects = await query.ApplyPagination(pagination).ToListAsync(ct);

        var paginatedList = projects.TransformToPageList(pagination, totalCount);

        return paginatedList;
    }

    public async Task<Project?> GetProjectDetailsByIdAsync(int id, CancellationToken ct)
    {
        return await _dbContext.Projects
            .Include(t => t.TodoItems)
            .FirstOrDefaultAsync( p => p.Id == id, ct);
    }

    public async Task<Project?> GetProjectByIdAsync(int id, CancellationToken ct)
    {
        return await _dbContext.Projects
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public void Remove(Project project)
    {
         _dbContext.Projects.Remove(project);
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await _dbContext.SaveChangesAsync(ct);
    }
}
