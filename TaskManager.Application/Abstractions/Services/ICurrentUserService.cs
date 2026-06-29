using System.Security.Claims;

namespace TaskManager.Application.Abstractions.Services
{
    public interface ICurrentUserService
    {
        int GetCurrentUserId();
    }
}