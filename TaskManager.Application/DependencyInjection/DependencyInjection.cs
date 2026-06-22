using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
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
    public static IServiceCollection AddAppicationServices(this IServiceCollection services)
    {
        services.AddScoped<ITodoService, TodoService>();
        services.AddScoped<IProjectService, ProjectService>();

        services.AddValidatorsFromAssemblyContaining<CreateProjectValidation>();
        services.AddValidatorsFromAssemblyContaining<CreateTodoRequestvalidation>();
        services.AddValidatorsFromAssemblyContaining<PaginationParamValidation>();

        services.AddAutoMapper(cfg => cfg.AddProfile<TodoProfile>());
        services.AddAutoMapper(cfg => cfg.AddProfile<ProjectProfile>());

        return services;
    }
}
