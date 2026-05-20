namespace ProjectTaskManagement.Application.DTOs.Auth;

public record AuthResponseDto(string Token, string Email, string FullName, IList<string> Roles);
