using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectTaskManagement.Application.Common;
using ProjectTaskManagement.Application.DTOs.Tasks;
using ProjectTaskManagement.Application.Interfaces;
using ProjectTaskManagement.Domain.Entities;
using ProjectTaskManagement.Domain.Exceptions;

namespace ProjectTaskManagement.Application.Features.Tasks.Commands.CreateTask;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, ApiResponse<TaskItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ICacheService _cache;

    public CreateTaskCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, ICacheService cache)
    {
        _context = context;
        _currentUser = currentUser;
        _cache = cache;
    }

    public async Task<ApiResponse<TaskItemDto>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken)
            ?? throw new NotFoundException(nameof(Project), request.ProjectId);

        if (project.UserId != _currentUser.UserId)
            throw new ForbiddenException();

        var task = new TaskItem
        {
            Title = request.Title,
            Description = request.Description,
            DueDate = request.DueDate,
            Priority = request.Priority,
            ProjectId = request.ProjectId
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync(cancellationToken);

        await _cache.RemoveAsync($"tasks:project:{request.ProjectId}", cancellationToken);

        return ApiResponse<TaskItemDto>.Ok(new TaskItemDto(
            task.Id, task.Title, task.Description, task.Status,
            task.DueDate, task.Priority, task.ProjectId, task.CreatedAt), "Task created.");
    }
}
