using MediatR;
using ProjectTaskManagement.Application.Common;
using ProjectTaskManagement.Application.DTOs.Tasks;
using ProjectTaskManagement.Domain.Enums;

namespace ProjectTaskManagement.Application.Features.Tasks.Commands.CreateTask;

public record CreateTaskCommand(string Title, string Description, DateTime? DueDate, TaskPriority Priority, Guid ProjectId)
    : IRequest<ApiResponse<TaskItemDto>>;
