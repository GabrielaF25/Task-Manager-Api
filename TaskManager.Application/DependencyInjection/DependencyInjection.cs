using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TaskManager.Application.Behaviors;
using TaskManager.Application.Features.Authentication.RefreshTokens;
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

        services.AddValidatorsFromAssemblyContaining<CreateProjectCommandValidation>();
        services.AddValidatorsFromAssemblyContaining<RefreshTokenCommandValidation>();
        services.AddValidatorsFromAssemblyContaining<LogoutUserCommandValidation>();
        services.AddValidatorsFromAssemblyContaining<CreateTodoCommand>();
        services.AddValidatorsFromAssemblyContaining<PaginationParamValidation>();
        services.AddValidatorsFromAssemblyContaining<CreateUserCommand>();

        services.AddAutoMapper(cfg => cfg.AddProfile<TodoProfile>());
        services.AddAutoMapper(cfg => cfg.AddProfile<ProjectProfile>());
        services.AddAutoMapper(cfg => cfg.AddProfile<UserProfile>());

        //behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

        return services;
    }
}
