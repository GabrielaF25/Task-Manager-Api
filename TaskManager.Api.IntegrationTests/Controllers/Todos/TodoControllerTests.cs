using System.Net;
using NUnit.Framework;
using TaskManager.Api.IntegrationTests.Common;

namespace TaskManager.Api.IntegrationTests.Controllers.Todos;

[TestFixture]
public class TodoControllerTests : IntegrationTestBase
{
    [Test]
    public async Task GetTodos_Should_Return_Response()
    {
        var response = await Client!.GetAsync("/api/todos");

        Assert.That(
            response.StatusCode,
            Is.Not.EqualTo(HttpStatusCode.Unauthorized));

        Assert.That(
            response.StatusCode,
            Is.Not.EqualTo(HttpStatusCode.Forbidden));
    }
}
