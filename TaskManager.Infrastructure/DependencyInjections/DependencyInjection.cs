using Microsoft.Extensions.DependencyInjection;
using Task_Manager_Api.Repository;
using TaskManager.Application.Abstraction;

namespace TaskManager.Infrastructure.DependencyInjections;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<ITodoRepository, TodoRepository>();

        return services;
    }
}
