using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectTaskManagement.Application.Common;
using ProjectTaskManagement.Application.DTOs.Tasks;
using ProjectTaskManagement.Application.Interfaces;
using ProjectTaskManagement.Domain.Exceptions;

namespace ProjectTaskManagement.Application.Features.Tasks.Queries.GetTasksByProject;

public class GetTasksByProjectQueryHandler : IRequestHandler<GetTasksByProjectQuery, ApiResponse<IEnumerable<TaskItemDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ICacheService _cache;

    public GetTasksByProjectQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser, ICacheService cache)
    {
        _context = context;
        _currentUser = currentUser;
        _cache = cache;
    }

    public async Task<ApiResponse<IEnumerable<TaskItemDto>>> Handle(GetTasksByProjectQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"tasks:project:{request.ProjectId}";

        var cached = await _cache.GetAsync<List<TaskItemDto>>(cacheKey, cancellationToken);
        if (cached is not null)
            return ApiResponse<IEnumerable<TaskItemDto>>.Ok(cached);

        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Project), request.ProjectId);

        if (project.UserId != _currentUser.UserId)
            throw new ForbiddenException();

        var tasks = await _context.Tasks
            .Where(t => t.ProjectId == request.ProjectId)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new TaskItemDto(t.Id, t.Title, t.Description, t.Status, t.DueDate, t.Priority, t.ProjectId, t.CreatedAt))
            .ToListAsync(cancellationToken);

        await _cache.SetAsync(cacheKey, tasks, TimeSpan.FromMinutes(5), cancellationToken);

        return ApiResponse<IEnumerable<TaskItemDto>>.Ok(tasks);
    }
}
