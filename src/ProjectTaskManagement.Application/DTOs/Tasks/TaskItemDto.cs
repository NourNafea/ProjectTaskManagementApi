using ProjectTaskManagement.Domain.Enums;

namespace ProjectTaskManagement.Application.DTOs.Tasks;

public record TaskItemDto(
    Guid Id,
    string Title,
    string Description,
    TaskItemStatus Status,
    DateTime? DueDate,
    TaskPriority Priority,
    Guid ProjectId,
    DateTime CreatedAt);
