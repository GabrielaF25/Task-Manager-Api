using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Common.Pagination;
using TaskManager.Application.Features.Projects.Dto;
using TaskManager.Application.Features.Projects.Queries;

namespace Task_Manager_Api.Controllers;

[Route("api/projects")]
[ApiController]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpPost]

    public async Task<ActionResult<ProjectDto>> CreateProject([FromBody] CreateProjectRequest request, CancellationToken ct)
    {
        var returnedProject = await _projectService.CreateProjectAsync(request, ct);

        return CreatedAtRoute("GetProjectById", new {id = returnedProject.Id}, returnedProject);
    }

    [HttpGet("{id}", Name = "GetProjectById")]
    public async Task<ActionResult<ProjectDto>> GetProjectByIdDetailedAsync(int id, CancellationToken ct)
    {
        var project = await _projectService.GetProjectDetailsByIdAsync(id, ct);

        if (project == null) 
        { 
            return NotFound();
        }

        return Ok(project);
    }

    [HttpGet]
    public async Task<ActionResult<PaginationResult<ProjectDto>>> 
        GetProjectsAsync([FromQuery] QueryParamProject queryParam,[FromQuery] PaginationParam pagination,CancellationToken ct)
    {
        var projects = await _projectService.GetProjectsAsync( queryParam, pagination, ct);

        return Ok(projects);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(int id, CancellationToken ct)
    {
        var status = await _projectService.RemoveAsync(id, ct);

        if (!status)
        {
            return NotFound();
        }

        return NoContent();
    }
}
