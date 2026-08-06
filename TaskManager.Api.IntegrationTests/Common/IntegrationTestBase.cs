namespace TaskManager.Api.IntegrationTests.Common;

public abstract class IntegrationTestBase
{
    protected HttpClient Client = null!;
    protected CustomWebApplicationFactory Factory = null!;

    [SetUp]
    public void SetUp()
    {
        Factory = new CustomWebApplicationFactory();
        Client = Factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        Client.Dispose();
        Factory.Dispose();
    }
}
