using MediatR;
using ProjectTaskManagement.Application.Common;

namespace ProjectTaskManagement.Application.Features.Tasks.Commands.DeleteTask;

public record DeleteTaskCommand(Guid TaskId) : IRequest<ApiResponse>;
