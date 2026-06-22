using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Common.Pagination;
using TaskManager.Application.Features.Projects.Dto;
using TaskManager.Application.Features.Projects.Queries;
using TaskManager.Domain.Entities;

namespace Task_Manager_Api.Controllers;

[Route("api/project")]
[ApiController]
public class ProjectController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectController(IProjectService project)
    {
        _projectService = project;
    }

    [HttpPost]

    public async Task<ActionResult<Project>> CreateProject([FromBody] CreateProjectRequest CreateProject, CancellationToken ct)
    {
        var returnedProject = await _projectService.CreateProjectAsync(CreateProject, ct);

        return Ok(returnedProject);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectDto>> GetProjectByIdDetailedAsync(int id, CancellationToken ct)
    {
        var project = await _projectService.GetProjectDetailsByIdAsync(id, ct);

        if (project == null) 
        { 
            NotFound();
        }

        return Ok(project);
    }

    [HttpGet]
    public async Task<ActionResult<PaginationResult<ProjectDto>>> 
        GetProjectsAsync([FromQuery] QueryParamProject queryParam,[FromQuery]PaginationParam pagination,CancellationToken ct)
    {
        var projects = await _projectService.GetProjectsAsync( queryParam, pagination, ct);

        return Ok(projects);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> RemoveAsync(int id, CancellationToken ct)
    {
        var status = await _projectService.RemoveAsync(id, ct);

        if (!status)
        {
            return BadRequest();
        }

        return NoContent();
    }
}
