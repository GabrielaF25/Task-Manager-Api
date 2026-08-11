using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.IntegrationTests.Common;

namespace TaskManager.Infrastructure.IntegrationTests.Repository.ProjectTests;

public class GetProjectByIdTests : ProjectBaseTest
{
    [Test]
    public async Task GetProjectById_Should_Return_Project()
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

        var returnedProject = await _projectRepository.GetProjectByIdAsync(project.Id, CancellationToken.None);

        // Assert

        Assert.That(returnedProject, Is.Not.Null);
        Assert.That(returnedProject.Id, Is.EqualTo(project.Id));
    }

    [Test]
    public async Task GetProjectById_Should_Return_Project_When_Exist_MoreData()
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

        var returnedProject = await _projectRepository.GetProjectByIdAsync(project.Id, CancellationToken.None);

        // Asser

        Assert.That(returnedProject, Is.Not.Null);
        Assert.That(returnedProject.Id, Is.EqualTo(project.Id));
    }
}
