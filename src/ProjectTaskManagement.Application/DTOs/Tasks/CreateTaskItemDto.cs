using ProjectTaskManagement.Domain.Enums;

namespace ProjectTaskManagement.Application.DTOs.Tasks;

public record CreateTaskItemDto(
    string Title,
    string Description,
    DateTime? DueDate,
    TaskPriority Priority,
    Guid ProjectId);
