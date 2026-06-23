using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Features.Projects.Mappings;
using TaskManager.Application.Features.Projects.Validation;
using TaskManager.Application.Features.Todo.Mapper;
using TaskManager.Application.Pagination.Validation;
using TaskManager.Application.Services;
using TaskManager.Domain.Features.Todo.Validations;

namespace TaskManager.Domain.DependencyInjections;

public static class ServiceCollection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ITodoService, TodoService>();

        services.AddValidatorsFromAssemblyContaining<CreateProjectValidation>();
        services.AddValidatorsFromAssemblyContaining<CreateTodoRequestvalidation>();
        services.AddValidatorsFromAssemblyContaining<PaginationParamValidation>();

        services.AddAutoMapper(cfg => cfg.AddProfile<TodoProfile>());
        services.AddAutoMapper(cfg => cfg.AddProfile<ProjectProfile>());

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

        return services;
    }
}
