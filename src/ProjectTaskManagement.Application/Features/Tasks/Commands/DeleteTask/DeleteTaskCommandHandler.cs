using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectTaskManagement.Application.Common;
using ProjectTaskManagement.Application.Interfaces;
using ProjectTaskManagement.Domain.Exceptions;

namespace ProjectTaskManagement.Application.Features.Tasks.Commands.DeleteTask;

public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, ApiResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ICacheService _cache;

    public DeleteTaskCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, ICacheService cache)
    {
        _context = context;
        _currentUser = currentUser;
        _cache = cache;
    }

    public async Task<ApiResponse> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _context.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.TaskItem), request.TaskId);

        if (task.Project.UserId != _currentUser.UserId)
            throw new ForbiddenException();

        var projectId = task.ProjectId;
        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync(cancellationToken);

        await _cache.RemoveAsync($"tasks:project:{projectId}", cancellationToken);

        return ApiResponse.Ok("Task deleted.");
    }
}
