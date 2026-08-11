using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.IntegrationTests.Common;

namespace TaskManager.Infrastructure.IntegrationTests.Repository.TodoTests;

public class DeleteTodoTests : TodoBaseTests
{
    [Test]
    public async Task RemoveAsync_Should_Delete()
    {
        // Arrange

        var user = User.Register("test@test.ro", "Test User");
        user.SetPasswordHash("hash");

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var project = Project.Create("Test Project", "Test Description", user.Id);

        await _dbContext.Projects.AddAsync(project);
        await _dbContext.SaveChangesAsync();

        var todoItem = TodoItem.Create("Todo Test1", "Todo Description1", project.Id);

        await _dbContext.TodoItems.AddAsync(todoItem);

        await _dbContext.SaveChangesAsync();
        // Act

         _todoRepository.Remove(todoItem);
        await _dbContext.SaveChangesAsync();
        // Assert

        var todoFromDb = await _dbContext.TodoItems.FirstOrDefaultAsync();

        Assert.Null(todoFromDb);
    }

    [Test]
    public async Task RemoveAsync_Should_Delete_When_MoreData()
    {
        // Arrange

        var user = User.Register("test@test.ro", "Test User");
        user.SetPasswordHash("hash");

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var project = Project.Create("Test Project", "Test Description", user.Id);

        await _dbContext.Projects.AddAsync(project);
        await _dbContext.SaveChangesAsync();

        await TestDataSeeder.SeedTodosAsync(_dbContext, project.Id);

        var todoItem = TodoItem.Create("Todo Test1", "Todo Description1", project.Id);

        await _dbContext.TodoItems.AddAsync(todoItem);

        await _dbContext.SaveChangesAsync();
        // Act

        _todoRepository.Remove(todoItem);
        await _dbContext.SaveChangesAsync();
        // Assert

        var todoFromDb = await _dbContext.TodoItems.FirstOrDefaultAsync(t => t.Id == todoItem.Id);

        Assert.That(todoFromDb, Is.Null);
        Assert.That(_dbContext.TodoItems.Count, Is.EqualTo(10));
    }
}
