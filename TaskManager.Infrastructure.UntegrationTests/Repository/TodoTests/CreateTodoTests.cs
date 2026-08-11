using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.IntegrationTests.Repository.TodoTests;

public class CreateTodoTests : TodoBaseTests
{
    [Test]
    public async Task AddAsync_Should_Add_Todo()
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

        // Act

        await _todoRepository.AddAsync(todoItem, CancellationToken.None);

        await _dbContext.SaveChangesAsync();

        // Assert

        var todoFromDb = await _dbContext.TodoItems.FirstOrDefaultAsync();

        Assert.NotNull(todoFromDb);

        Assert.That(todoFromDb.Title, Is.EqualTo(todoItem.Title));
        Assert.That(todoFromDb.Description, Is.EqualTo(todoItem.Description));
    }
}
