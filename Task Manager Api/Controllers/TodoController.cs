using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Common.Pagination;
using TaskManager.Application.Features.Todos.CreateTodo;
using TaskManager.Application.Features.Todos.DeleteTodo;
using TaskManager.Application.Features.Todos.Dtos;
using TaskManager.Application.Features.Todos.GetTodos;
using TaskManager.Application.Features.Todos.Queries.GetTodo;
using TaskManager.Application.Features.Todos.UpdateTodo;

namespace Task_Manager_Api.Controllers;

[Route("api/todos")]
[ApiController]
[Authorize(Roles = "Admin")]
public class TodoController : BaseController
{
    private readonly IMediator _mediator;

    public TodoController(IMediator mediator)
    {
       _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PaginationResult<TodoResponse>>>
        GetTodoItemsAsync([FromQuery] QueryParamTodo queryParam, [FromQuery] PaginationParam pagination, CancellationToken ct)
    {
        var pagedData = await _mediator.Send(new GetTodosQuery(queryParam, pagination), ct);
        return HandleResult(pagedData);
    }

    [HttpGet("{id}", Name = "GetById")]
    public async Task<ActionResult<TodoResponse>> GetItemAsync(int id, CancellationToken ct)
    {
        var item = await _mediator.Send(new GetTodoQuery(id), ct);

       return  HandleResult(item);

    }

    [HttpPost]
    public async Task<ActionResult<TodoResponse>> AddItemAsync([FromBody] CreateTodoRequest item, CancellationToken ct)
    {
        var response = await _mediator.Send(new CreateTodoCommand(item), ct);
        return HandleCreatedResult("GetById", response, dto => new {id = dto.Id});
    }

    [HttpPatch("{id}/complete")]
    public async Task<ActionResult<TodoResponse>> UpdateItemStatusAsync(int id, CancellationToken ct)
    {
        var response = await _mediator.Send(new UpdateTodoCommand(id), ct);

        return HandleResult(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteItemAsync(int id, CancellationToken ct)
    {
        var response = await _mediator.Send(new DeleteTodoCommand(id), ct);
        return HandleResult(response);
    }
}
