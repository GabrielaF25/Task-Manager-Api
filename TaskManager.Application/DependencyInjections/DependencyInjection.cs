using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Task_Manager_Api.Services;
using TaskManager.Application.Features.Todo.Profiles;
using TaskManager.Application.Features.Todo.Validations;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.DependencyInjections;

public static class ServiceCollection
{
    public static IServiceCollection AddAppicationServices(this IServiceCollection services)
    {
        services.AddScoped<ITodoService, TodoService>();
        services.AddValidatorsFromAssemblyContaining<CreateTodoRequestvalidation>();
        services.AddAutoMapper(cfg => cfg.AddProfile<TodoProfile>());

        return services;
    }
}
