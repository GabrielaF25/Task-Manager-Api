using Microsoft.AspNetCore.Mvc;
using Task_Manager_Api.Dtos;
using Task_Manager_Api.Services;

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
    public async Task<ActionResult<IEnumerable<TodoResponse>>> GetTodoItemsAsync()
    {
        return Ok(await _todoService.GetAllAsync());
    }

    [HttpGet("{id}", Name = "GetById")]
    public async Task<ActionResult<TodoResponse?>> GetItemAsync(int id)
    {
        var item = await _todoService.GetByIdAsync(id);

        if (item == null)
        {
            return NotFound();
        }

        return Ok(item);

    }

    [HttpPost]
    public async Task<ActionResult<TodoResponse>> AddItemAsync([FromBody] CreateTodoRequest item)
    {
        var response = await _todoService.AddItemAsync(item);
        return CreatedAtRoute("GetById", new { id = response.Id}, response);
    }

    [HttpPatch("{id}/complete")]
    public async Task<ActionResult<TodoResponse>> UpdateItemStatusAsync(int id)
    {
        var response = await _todoService.UpdateItemStatusAsync(id);

        if(response == null)
        {
            return NotFound();
        }
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteItemAsync(int id)
    {
        var response = await _todoService.DeleteItem(id);
        if (!response)
        {
            return NotFound();
        }
        return NoContent();
    }
}
