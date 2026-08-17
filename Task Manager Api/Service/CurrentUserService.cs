using System.Security.Claims;
using TaskManager.Application.Abstractions.Services;
using TaskManager.Application.Common.ResultPattern;

namespace Task_Manager_Api.Service;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContext;

    public CurrentUserService(IHttpContextAccessor httpContext)
    {
        _httpContext = httpContext;
    }

    public Guid GetCurrentUserId()
    {
        var userId = _httpContext.HttpContext?
            .User.FindFirstValue(ClaimTypes.NameIdentifier);

        if(!Guid.TryParse(userId, out Guid id))
        {
            throw new UnauthorizedAccessException();
        }

        return id;
    }
}
