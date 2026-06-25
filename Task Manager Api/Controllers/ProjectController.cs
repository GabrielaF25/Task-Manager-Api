using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Common.Pagination;
using TaskManager.Application.Features.Projects.CreateProject;
using TaskManager.Application.Features.Projects.DeleteProject;
using TaskManager.Application.Features.Projects.Dto;
using TaskManager.Application.Features.Projects.GetProjectDetails;
using TaskManager.Application.Features.Projects.GetProjects;

namespace Task_Manager_Api.Controllers;

[Route("api/projects")]
[ApiController]
public class ProjectController : BaseController
{
    private readonly IMediator _mediator;
    public ProjectController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<ProjectDto>> CreateProject([FromBody] CreateProjectRequest request, CancellationToken ct)
    {
        var returnedProjectResult = await _mediator.Send(new CreateProjectCommand(request), ct);

        return HandleCreatedResult("GetProjectById", returnedProjectResult, dto => new {id = dto.Id });
    }

    [HttpGet("{id}", Name = "GetProjectById")]
    public async Task<ActionResult<ProjectDto>> GetProjectByIdDetailedAsync(int id, CancellationToken ct)
    {
        var project = await _mediator.Send(new GetProjectDetailsQuery(id), ct);

        return HandleResult(project);
    }

    [HttpGet]
    public async Task<ActionResult<PaginationResult<ProjectDto>>> 
        GetProjectsAsync([FromQuery] QueryParamProject queryParam,[FromQuery] PaginationParam pagination,CancellationToken ct)
    {
        var projects = await _mediator.Send(new GetProjectQuery( queryParam, pagination), ct);

        return HandleResult(projects);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new  DeleteProjectCommand(id), ct);

        return HandleResult(result);
    }
}
