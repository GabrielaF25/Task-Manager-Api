using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.DbContexts;

namespace TaskManager.Infrastructure.IntegrationTests.Common;

public class TestDataSeeder
{
    public static async Task<User> SeedUserAsync(
        TaskManagerDbContext dbContext,
        string email = "user@test.com",
        string userName = "TestUser")
    {
        var user = User.Register(email, userName);
        user.SetPasswordHash("hash");

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        return user;
    }

    public static async Task<List<Project>> SeedProjectsAsync(
        TaskManagerDbContext dbContext,
        int ownerId,
        int count = 10)
    {
        var projects = new List<Project>();

        for (var i = 1; i <= count; i++)
        {
            var project = Project.Create(
                $"Project {i}",
                $"Description {i}",
                ownerId);

            projects.Add(project);
        }

        dbContext.Projects.AddRange(projects);
        await dbContext.SaveChangesAsync();

        return projects;
    }

    public static async Task<List<TodoItem>> SeedTodosAsync(
        TaskManagerDbContext dbContext,
        int projectId,
        int count = 10)
    {
        var todos = new List<TodoItem>();

        for (var i = 1; i <= count; i++)
        {
            var todo = TodoItem.Create(
                $"Todo {i}",
                $"Description {i}",
                projectId);

            todos.Add(todo);
        }

        dbContext.TodoItems.AddRange(todos);
        await dbContext.SaveChangesAsync();

        return todos;
    }
}
