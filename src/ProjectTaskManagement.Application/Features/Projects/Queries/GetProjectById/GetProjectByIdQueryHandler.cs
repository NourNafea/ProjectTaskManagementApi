using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectTaskManagement.Application.Common;
using ProjectTaskManagement.Application.DTOs.Projects;
using ProjectTaskManagement.Application.Interfaces;
using ProjectTaskManagement.Domain.Exceptions;

namespace ProjectTaskManagement.Application.Features.Projects.Queries.GetProjectById;

public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ApiResponse<ProjectDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ICacheService _cache;

    public GetProjectByIdQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser, ICacheService cache)
    {
        _context = context;
        _currentUser = currentUser;
        _cache = cache;
    }

    public async Task<ApiResponse<ProjectDto>> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        // Key is user-scoped so no cross-user cache leak is possible
        var cacheKey = $"project:{_currentUser.UserId}:{request.Id}";

        var cached = await _cache.GetAsync<ProjectDto>(cacheKey, cancellationToken);
        if (cached is not null)
            return ApiResponse<ProjectDto>.Ok(cached);

        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Project), request.Id);

        if (project.UserId != _currentUser.UserId)
            throw new ForbiddenException();

        var dto = new ProjectDto(project.Id, project.Name, project.Description, project.CreatedAt);
        await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(5), cancellationToken);

        return ApiResponse<ProjectDto>.Ok(dto);
    }
}
