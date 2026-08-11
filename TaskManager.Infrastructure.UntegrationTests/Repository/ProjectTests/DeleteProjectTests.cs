using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.IntegrationTests.Common;

namespace TaskManager.Infrastructure.IntegrationTests.Repository.ProjectTests;

public class DeleteProjectTests : ProjectBaseTest
{
    [Test]
    public async Task Remove_Should_Delete_Project()
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

         _projectRepository.Remove(project);

        await _dbContext.SaveChangesAsync();

        // Assert

        var projectFromDb = await _dbContext.Projects.FirstOrDefaultAsync();

        Assert.That(projectFromDb, Is.Null);
    }

    [Test]
    public async Task Remove_Async_Should_Delete_Project_When_Exist_MoreData()
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

        // Act

        _projectRepository.Remove(project);

        await _dbContext.SaveChangesAsync();

        // Assert

        var projectFromDb = await _dbContext.Projects.FirstOrDefaultAsync(p => p.Id == project.Id);
        var countProjects =  _dbContext.Projects.Count();

        Assert.That(projectFromDb, Is.Null);
        Assert.That(countProjects, Is.EqualTo(10));
    }
}
