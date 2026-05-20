using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectTaskManagement.Application.Common;
using ProjectTaskManagement.Application.DTOs.Projects;
using ProjectTaskManagement.Application.Interfaces;

namespace ProjectTaskManagement.Application.Features.Projects.Queries.GetAllProjects;

public class GetAllProjectsQueryHandler : IRequestHandler<GetAllProjectsQuery, ApiResponse<IEnumerable<ProjectDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ICacheService _cache;

    public GetAllProjectsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser, ICacheService cache)
    {
        _context = context;
        _currentUser = currentUser;
        _cache = cache;
    }

    public async Task<ApiResponse<IEnumerable<ProjectDto>>> Handle(GetAllProjectsQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"projects:user:{_currentUser.UserId}";

        var cached = await _cache.GetAsync<List<ProjectDto>>(cacheKey, cancellationToken);
        if (cached is not null)
            return ApiResponse<IEnumerable<ProjectDto>>.Ok(cached);

        var projects = await _context.Projects
            .Where(p => p.UserId == _currentUser.UserId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new ProjectDto(p.Id, p.Name, p.Description, p.CreatedAt))
            .ToListAsync(cancellationToken);

        await _cache.SetAsync(cacheKey, projects, TimeSpan.FromMinutes(5), cancellationToken);

        return ApiResponse<IEnumerable<ProjectDto>>.Ok(projects);
    }
}
