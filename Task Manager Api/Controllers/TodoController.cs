using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Features.Todo.Dtos;
using TaskManager.Application.Interfaces;
using TaskManager.Application.Pagination;
using TaskManager.Domain.Pagination;

namespace Task_Manager_Api.Controllers;

[Route("api/todos")]
[ApiController]
public class TodoController : ControllerBase
{
    private readonly ITodoService _todoService;

    public TodoController(ITodoService todoService)
    {
        _todoService = todoService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoResponse>>>
        GetTodoItemsAsync([FromQuery] QueryParamTodo queryParam, [FromQuery] PaginationParam pagination, CancellationToken ct)
    {
        return Ok(await _todoService.GetAllAsync(queryParam, pagination, ct));
    }

    [HttpGet("{id}", Name = "GetById")]
    public async Task<ActionResult<TodoResponse?>> GetItemAsync(int id, CancellationToken ct)
    {
        var item = await _todoService.GetByIdAsync(id, ct);

        if (item == null)
        {
            return NotFound();
        }

        return Ok(item);

    }

    [HttpPost]
    public async Task<ActionResult<TodoResponse>> AddItemAsync([FromBody] CreateTodoRequest item, CancellationToken ct)
    {
        var response = await _todoService.AddItemAsync(item, ct);
        return CreatedAtRoute("GetById", new { id = response.Id}, response);
    }

    [HttpPatch("{id}/complete")]
    public async Task<ActionResult<TodoResponse>> UpdateItemStatusAsync(int id, CancellationToken ct)
    {
        var response = await _todoService.UpdateItemStatusAsync(id, ct);

        if(response == null)
        {
            return NotFound();
        }
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteItemAsync(int id, CancellationToken ct)
    {
        var response = await _todoService.DeleteItemAsync(id, ct);
        if (!response)
        {
            return NotFound();
        }
        return NoContent();
    }
}
