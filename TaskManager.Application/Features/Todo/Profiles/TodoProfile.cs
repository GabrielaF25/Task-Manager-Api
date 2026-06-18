using AutoMapper;
using Task_Manager_Api.Models;
using TaskManager.Application.Features.Todo.Dtos;
namespace TaskManager.Application.Features.Todo.Profiles;

public class TodoProfile :Profile
{
    public TodoProfile()
    {
        CreateMap<TodoItem, TodoResponse>().ReverseMap();
        CreateMap<CreateTodoRequest, TodoItem>();
    }
}
