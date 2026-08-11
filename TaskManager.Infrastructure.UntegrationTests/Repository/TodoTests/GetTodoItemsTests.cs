using TaskManager.Application.Common.Pagination;
using TaskManager.Application.Features.Todos.GetTodos;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.IntegrationTests.Common;

namespace TaskManager.Infrastructure.IntegrationTests.Repository.TodoTests;

public class GetTodoItemsTests : TodoBaseTests
{
    [Test]

    public async Task Should_Get_Todos()
    {
        // Arrange

        var user = User.Register("test@test.ro", "Test User");

        user.SetPasswordHash("Hash");

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var project = Project.Create("Test", "Test Description", user.Id);

        await _dbContext.Projects.AddAsync(project);
        await _dbContext.SaveChangesAsync();

        await TestDataSeeder.SeedTodosAsync(_dbContext, project.Id);

        // Act

        var paginatedTodos = await _todoRepository.GetTodosAsync(
            new QueryParamTodo(),
            new PaginationParam(),
            CancellationToken.None);

        // Assert

        Assert.That(paginatedTodos, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(paginatedTodos.TotalCount, Is.EqualTo(10));
            Assert.That(paginatedTodos.HasPreviousPage, Is.False);
            Assert.That(paginatedTodos.HasNextPage, Is.False);
            Assert.That(paginatedTodos.Items.Count, Is.EqualTo(10));
            Assert.That(paginatedTodos.TotalPages, Is.EqualTo(1));
        });
    }
}
