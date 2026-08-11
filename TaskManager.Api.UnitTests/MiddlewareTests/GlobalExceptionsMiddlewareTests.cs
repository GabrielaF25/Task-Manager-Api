using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Text.Json;
using Task_Manager_Api.Middlewares;

namespace TaskManager.Api.UnitTests.MiddlewareTests;

public class GlobalExceptionsMiddlewareTests
{

    private Mock<ILogger<GlobalExceptionMiddleware>> _loggerMock = null!;
    private GlobalExceptionMiddleware _middleware = null!;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<GlobalExceptionMiddleware>>();
        _middleware = new GlobalExceptionMiddleware(_loggerMock.Object);
    }

    [Test]

    public async Task TryHandleAsync_Shoul_Return_Unauthorized_When_Exception_Is_UnauthorizedAccessException()
    {
        // Arrange

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = "/api/projects";
        httpContext.Response.Body = new MemoryStream();

        var exception = new UnauthorizedAccessException();

        // Act
        var handled = await _middleware.TryHandleAsync(
            httpContext,
            exception,
            CancellationToken.None);

        // Assert

        Assert.That(handled, Is.True);
        Assert.That(httpContext.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.Unauthorized));

        httpContext.Response.Body.Position = 0;

        var problemDetails = await JsonSerializer.DeserializeAsync<ProblemDetails>(httpContext.Response.Body, 
            new JsonSerializerOptions(JsonSerializerDefaults.Web));

        Assert.That(problemDetails, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(problemDetails.Status, Is.EqualTo(401));
            Assert.That(problemDetails.Title, Is.EqualTo("Unauthorized"));
            Assert.That(problemDetails.Detail, Is.EqualTo("An error ouccurred"));
            Assert.That(problemDetails.Instance, Is.EqualTo("/api/projects"));
        });
    }

    [Test]
    public async Task TryHandleAsync_Should_Return_InternalServerError_When_Exception_Is_Unknown()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = "/api/todos";
        httpContext.Response.Body = new MemoryStream();

        var exception = new InvalidOperationException("Something failed");

        // Act
        var handled = await _middleware.TryHandleAsync(
            httpContext,
            exception,
            CancellationToken.None);

        // Assert
        Assert.That(handled, Is.True);
        Assert.That(
            httpContext.Response.StatusCode,
            Is.EqualTo((int)HttpStatusCode.InternalServerError));

        httpContext.Response.Body.Position = 0;

        var problemDetails = await JsonSerializer.DeserializeAsync<ProblemDetails>(
            httpContext.Response.Body,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));

        Assert.That(problemDetails, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(problemDetails!.Status, Is.EqualTo(500));
            Assert.That(problemDetails.Title, Is.EqualTo("Internal Server Error"));
            Assert.That(problemDetails.Detail, Is.EqualTo("An error ouccurred"));
            Assert.That(problemDetails.Instance, Is.EqualTo("/api/todos"));
        });
    }
}
