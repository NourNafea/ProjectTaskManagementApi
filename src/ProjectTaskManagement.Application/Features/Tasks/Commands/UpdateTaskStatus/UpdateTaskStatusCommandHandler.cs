using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectTaskManagement.Application.Common;
using ProjectTaskManagement.Application.DTOs.Tasks;
using ProjectTaskManagement.Application.Interfaces;
using ProjectTaskManagement.Domain.Exceptions;

namespace ProjectTaskManagement.Application.Features.Tasks.Commands.UpdateTaskStatus;

public class UpdateTaskStatusCommandHandler : IRequestHandler<UpdateTaskStatusCommand, ApiResponse<TaskItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ICacheService _cache;

    public UpdateTaskStatusCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, ICacheService cache)
    {
        _context = context;
        _currentUser = currentUser;
        _cache = cache;
    }

    public async Task<ApiResponse<TaskItemDto>> Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
    {
        var task = await _context.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.TaskItem), request.TaskId);

        if (task.Project.UserId != _currentUser.UserId)
            throw new ForbiddenException();

        task.Status = request.Status;
        await _context.SaveChangesAsync(cancellationToken);

        await _cache.RemoveAsync($"tasks:project:{task.ProjectId}", cancellationToken);

        return ApiResponse<TaskItemDto>.Ok(new TaskItemDto(
            task.Id, task.Title, task.Description, task.Status,
            task.DueDate, task.Priority, task.ProjectId, task.CreatedAt));
    }
}
