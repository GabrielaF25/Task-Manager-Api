using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TaskManager.Application.Features.Projects.CreateProject;
using TaskManager.Application.Features.Projects.Mappings;
using TaskManager.Application.Features.Todo.Mapper;
using TaskManager.Application.Features.Todos.CreateTodo;
using TaskManager.Application.Features.Users.CreateUser;
using TaskManager.Application.Features.Users.Mappings;
using TaskManager.Application.Pagination.Validation;

namespace TaskManager.Domain.DependencyInjections;

public static class ServiceCollection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {

        services.AddValidatorsFromAssemblyContaining<CreateProjectValidation>();
        services.AddValidatorsFromAssemblyContaining<CreateTodoRequestvalidation>();
        services.AddValidatorsFromAssemblyContaining<PaginationParamValidation>();
        services.AddValidatorsFromAssemblyContaining<CreateUserValidation>();

        services.AddAutoMapper(cfg => cfg.AddProfile<TodoProfile>());
        services.AddAutoMapper(cfg => cfg.AddProfile<ProjectProfile>());
        services.AddAutoMapper(cfg => cfg.AddProfile<UserProfile>());

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

        return services;
    }
}
