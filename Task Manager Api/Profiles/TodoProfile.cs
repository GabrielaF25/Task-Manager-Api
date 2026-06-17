using AutoMapper;
using Task_Manager_Api.Dtos;
using Task_Manager_Api.Models;
namespace Task_Manager_Api.Profiles;

public class TodoProfile :Profile
{
    public TodoProfile()
    {
        CreateMap<TodoItem, TodoResponse>().ReverseMap();
        CreateMap<CreateTodoRequest, TodoItem>();
    }
}
