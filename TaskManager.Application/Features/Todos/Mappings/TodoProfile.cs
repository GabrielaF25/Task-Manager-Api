using AutoMapper;
using TaskManager.Application.Features.Todos.Dtos;
using TaskManager.Domain.Entities;
namespace TaskManager.Application.Features.Todo.Mapper;

public class TodoProfile : Profile
{
    public TodoProfile()
    {
        CreateMap<TodoItem, TodoResponse>().ReverseMap();
        CreateMap<CreateTodoRequest, TodoItem>();
    }
}
