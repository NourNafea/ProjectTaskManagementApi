namespace ProjectTaskManagement.Application.Interfaces;

public interface IIdentityService
{
    Task<(bool Succeeded, string[] Errors, string UserId, string Email, string FullName, IList<string> Roles)> RegisterAsync(string fullName, string email, string password);
    Task<(bool Succeeded, string UserId, string Email, string FullName, IList<string> Roles)> LoginAsync(string email, string password);
}
