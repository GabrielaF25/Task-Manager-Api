using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.IntegrationTests.Repository.UserTests;

public class RegisterUserTests : UserBaseTests
{
    [Test]
    public async Task CreateUserAsync_Should_AddUser()
    {
        // Arrange
        var user = User.Register("Test@test.ro", "Test");
        user.SetPasswordHash("Hash");

        // Act

        await _userRepository.CreateUserAsync(user, CancellationToken.None);
        await _dbContext.SaveChangesAsync();

        // Assert

        var userFromDb = await _dbContext.Users.FirstOrDefaultAsync();

        Assert.NotNull(userFromDb);

        Assert.Multiple(() =>
        {
            Assert.That(userFromDb.PasswordHash, Is.EqualTo(user.PasswordHash));
            Assert.That(userFromDb.UserName, Is.EqualTo(user.UserName));
            Assert.That(userFromDb.Email, Is.EqualTo(user.Email));
            Assert.That(userFromDb.UserRole, Is.EqualTo(user.UserRole));

        });
    }
}
