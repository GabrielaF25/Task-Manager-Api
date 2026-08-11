using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.IntegrationTests.Repository.UserTests;

public class GetUserByEmail : UserBaseTests
{
    [Test]
    public async Task GetUserByEmail_Should_Return_User()
    {
        // Arrange

        var user = User.Register("test@test.ro", "Test");
        user.SetPasswordHash("hash");

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        // Act

        var response = await _userRepository.GetUserByUseEmail("test@test.ro", CancellationToken.None);

        // Assert


        Assert.NotNull(response);
        Assert.Multiple(() =>
        {
            Assert.That(response.UserName, Is.EqualTo(user.UserName));
            Assert.That(response.Email, Is.EqualTo(user.Email));
        });
    }
}
