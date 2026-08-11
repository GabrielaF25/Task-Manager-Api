using TaskManager.Application.Common.Pagination;
using TaskManager.Application.Features.Projects.GetProjects;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.IntegrationTests.Common;

namespace TaskManager.Infrastructure.IntegrationTests.Repository.ProjectTests;

public class GetProjectsTests : ProjectBaseTest 
{
    [Test]

    public async Task Should_Get_Projects()
    {
        // Arrange

        var user = User.Register("test@test.ro", "Test User");

        user.SetPasswordHash("Hash");

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        await TestDataSeeder.SeedProjectsAsync(_dbContext, user.Id);

        // Act

        var paginatedProjects = await _projectRepository.GetProjectsAsync(
            new QueryParamProject(), 
            user.Id, 
            new PaginationParam(), 
            CancellationToken.None);

        // Assert

        Assert.That(paginatedProjects, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(paginatedProjects.TotalCount, Is.EqualTo(10));
            Assert.That(paginatedProjects.HasPreviousPage, Is.False);
            Assert.That(paginatedProjects.HasNextPage, Is.False);
            Assert.That(paginatedProjects.Items.Count, Is.EqualTo(10));
            Assert.That(paginatedProjects.TotalPages, Is.EqualTo(1));
        });
    }
}
