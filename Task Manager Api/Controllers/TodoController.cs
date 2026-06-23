using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Common.Pagination;
using TaskManager.Application.Features.Todo.Queries;
using TaskManager.Application.Features.Todos.Dtos;

namespace Task_Manager_Api.Controllers;

[Route("api/todos")]
[ApiController]
public class TodoController : BaseController
{
    private readonly ITodoService _todoService;

    public TodoController(ITodoService todoService)
    {
        _todoService = todoService;
    }

    [HttpGet]
    public async Task<ActionResult<PaginationResult<TodoResponse>>>
        GetTodoItemsAsync([FromQuery] QueryParamTodo queryParam, [FromQuery] PaginationParam pagination, CancellationToken ct)
    {
        var pagedData = await _todoService.GetAllAsync(queryParam, pagination, ct);
        return HandleResult(pagedData);
    }

    [HttpGet("{id}", Name = "GetById")]
    public async Task<ActionResult<TodoResponse>> GetItemAsync(int id, CancellationToken ct)
    {
        var item = await _todoService.GetByIdAsync(id, ct);

       return  HandleResult(item);

    }

    [HttpPost]
    public async Task<ActionResult<TodoResponse>> AddItemAsync([FromBody] CreateTodoRequest item, CancellationToken ct)
    {
        var response = await _todoService.AddItemAsync(item, ct);
        return HandleCreatedResult("GetById", response, dto => new {id = dto.Id});
    }

    [HttpPatch("{id}/complete")]
    public async Task<ActionResult<TodoResponse>> UpdateItemStatusAsync(int id, CancellationToken ct)
    {
        var response = await _todoService.UpdateItemStatusAsync(id, ct);

        return HandleResult(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteItemAsync(int id, CancellationToken ct)
    {
        var response = await _todoService.DeleteItemAsync(id, ct);
        return HandleResult(response);
    }
}
