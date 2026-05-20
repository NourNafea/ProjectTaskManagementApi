using ProjectTaskManagement.Domain.Enums;

namespace ProjectTaskManagement.Application.DTOs.Tasks;

public record UpdateTaskStatusDto(TaskItemStatus Status);
