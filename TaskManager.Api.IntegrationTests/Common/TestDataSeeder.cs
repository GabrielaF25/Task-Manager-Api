using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.DbContexts;

namespace TaskManager.Api.IntegrationTests.Common;

public static class TestDataSeeder
{
    public static async Task<User> SeedAdminAsync(TaskManagerDbContext context) {

        var user = User.Register("test@test.com", "Test");
        await context.AddAsync(user);
        await context.SaveChangesAsync();
        return user;
    }

    public static async Task<Project> SeedProjectAsync(TaskManagerDbContext context) {

        var project = Project.Create("Test Project","Test description",1);
        await context.AddAsync(project);
        await context.SaveChangesAsync();
        return project;
    }

    public static async Task<TodoItem> SeedTodoAsync(TaskManagerDbContext context) 
    {

        var todoItem = TodoItem.Create("Test Title", "Test Description", 1);
        await context.AddAsync(todoItem);
        await context.SaveChangesAsync();
        return todoItem;
    }
}
