using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.IntegrationTests.Common;

namespace TaskManager.Infrastructure.IntegrationTests.Repository.ProjectTests;

public class GetProjectDetailesByIdTests : ProjectBaseTest
{
    [Test]
    public async Task GetProjectDetailsById_Should_Return_Project()
    {
        // Arrange

        var user = User.Register("test@test.ro", "test");
        user.SetPasswordHash("hash");

        await _dbContext.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var project = Project.Create("Test Project", "Descriptio  Test", user.Id);

        await _dbContext.Projects.AddAsync(project);
        await _dbContext.SaveChangesAsync();

        // Act

        var returnedProject = await _projectRepository.GetProjectDetailsByIdAsync(project.Id, CancellationToken.None);

        // Assert

        Assert.That(returnedProject, Is.Not.Null);
        Assert.That(returnedProject.Id, Is.EqualTo(project.Id));
        Assert.That(returnedProject.TodoItems, Is.Empty);
    }

    [Test]
    public async Task GetProjectDetailsById_Should_Return_Project_When_Exist_MoreData()
    {
        // Arrange

        var user = User.Register("test@test.ro", "test");
        user.SetPasswordHash("hash");

        await _dbContext.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        await TestDataSeeder.SeedProjectsAsync(_dbContext, user.Id);

        var project = Project.Create("Test Project", "Descriptio  Test", user.Id);

        await _dbContext.Projects.AddAsync(project);
        await _dbContext.SaveChangesAsync();

        var itemTodo1 = TodoItem.Create("Item1", "Description1", project.Id);
        var itemTodo2 = TodoItem.Create("Item2", "Description2", project.Id);


        await _dbContext.TodoItems.AddRangeAsync(itemTodo1, itemTodo2);
        await _dbContext.SaveChangesAsync();
        // Act

        var returnedProject = await _projectRepository.GetProjectDetailsByIdAsync(project.Id, CancellationToken.None);

        // Asser

        Assert.That(returnedProject, Is.Not.Null);
        Assert.That(returnedProject.Id, Is.EqualTo(project.Id));
        Assert.That(returnedProject.TodoItems, Has.Count.EqualTo(2));
    }
}
