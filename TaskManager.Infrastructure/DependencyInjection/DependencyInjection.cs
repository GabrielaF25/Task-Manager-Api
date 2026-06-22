using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Infrastructure.Repository;

namespace TaskManager.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<ITodoRepository, TodoRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();

        return services;
    }
}
