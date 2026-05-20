namespace ProjectTaskManagement.Application.DTOs.Admin;

public record UserDto(string Id, string Email, string FullName, IList<string> Roles);
