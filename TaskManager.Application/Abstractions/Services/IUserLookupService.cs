namespace TaskManager.Application.Abstractions.Services
{
    public interface IUserLookupService
    {
        Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken);
    }
}