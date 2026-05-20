namespace ProjectTaskManagement.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(string userId, string email, string fullName, IList<string> roles);
}
