using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.IntegrationTests.Common;

namespace TaskManager.Infrastructure.IntegrationTests.Repository.TodoTests;

public class GetTodoByIdTests : TodoBaseTests
{
    [Test]
    public async Task GetTodoById_Should_Return_Todo()
    {
        // Arrange

        var user = User.Register("test@test.ro", "test");
        user.SetPasswordHash("hash");

        await _dbContext.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var project = Project.Create("Test Todo", "Descriptio  Test", user.Id);

        await _dbContext.Projects.AddAsync(project);
        await _dbContext.SaveChangesAsync();

        var todo = TodoItem.Create("Test Title", "Test Description", project.Id);

        await _dbContext.TodoItems.AddAsync(todo);
        await _dbContext.SaveChangesAsync();

        // Act

        var returnedTodo = await _todoRepository.GetByIdAsync(project.Id, CancellationToken.None);

        // Assert

        Assert.That(returnedTodo, Is.Not.Null);
        Assert.That(returnedTodo.Id, Is.EqualTo(project.Id));
    }

    [Test]
    public async Task GetTodoById_Should_Return_Todo_When_Exist_MoreData()
    {
        // Arrange

        var user = User.Register("test@test.ro", "test");
        user.SetPasswordHash("hash");

        await _dbContext.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var project = Project.Create("Test Todo", "Descriptio  Test", user.Id);
        await _dbContext.Projects.AddAsync(project);
        await _dbContext.SaveChangesAsync();

        var todo = TodoItem.Create("Test Title", "Test Description", project.Id);
        await _dbContext.TodoItems.AddAsync(todo);
        await _dbContext.SaveChangesAsync();

        await TestDataSeeder.SeedTodosAsync(_dbContext, project.Id);
        // Act

        var returnedTodo = await _todoRepository.GetByIdAsync(todo.Id, CancellationToken.None);

        // Asser

        Assert.That(returnedTodo, Is.Not.Null);
        Assert.That(returnedTodo.Id, Is.EqualTo(todo.Id));
    }
}
