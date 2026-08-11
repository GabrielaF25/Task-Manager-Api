using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.IntegrationTests.Repository.ProjectTests;

public class CreateProjectTests : ProjectBaseTest
{
    [Test]
    public async Task AddAsync_Should_Add_Project()
    {
        // Arrange

        var user = User.Register("test@test.ro", "Test");

        user.SetPasswordHash("hash");

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var project = Project.Create("Test Project", "Test Description", user.Id);

        // Act

        var addedProject = await _projectRepository.AddAsync(project, CancellationToken.None);

        await _dbContext.SaveChangesAsync();

        // Assert

        var projectFromDb = await _dbContext.Projects.FirstOrDefaultAsync();

        Assert.That(projectFromDb, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(projectFromDb.Name, Is.EqualTo(project.Name));
            Assert.That(projectFromDb.Description, Is.EqualTo(project.Description));
            Assert.That(projectFromDb.OwnerId, Is.EqualTo(user.Id));
        });
    }
}
