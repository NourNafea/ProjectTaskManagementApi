namespace ProjectTaskManagement.Application.DTOs.Projects;

public record ProjectDto(Guid Id, string Name, string Description, DateTime CreatedAt);
