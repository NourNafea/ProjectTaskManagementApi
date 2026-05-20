using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectTaskManagement.Application.Common;
using ProjectTaskManagement.Application.DTOs.Projects;
using ProjectTaskManagement.Application.Interfaces;
using ProjectTaskManagement.Domain.Exceptions;

namespace ProjectTaskManagement.Application.Features.Projects.Commands.UpdateProject;

public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, ApiResponse<ProjectDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ICacheService _cache;

    public UpdateProjectCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, ICacheService cache)
    {
        _context = context;
        _currentUser = currentUser;
        _cache = cache;
    }

    public async Task<ApiResponse<ProjectDto>> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Project), request.Id);

        if (project.UserId != _currentUser.UserId)
            throw new ForbiddenException();

        project.Name = request.Name;
        project.Description = request.Description;

        await _context.SaveChangesAsync(cancellationToken);

        await Task.WhenAll(
            _cache.RemoveAsync($"projects:user:{_currentUser.UserId}", cancellationToken),
            _cache.RemoveAsync($"project:{_currentUser.UserId}:{request.Id}", cancellationToken));

        return ApiResponse<ProjectDto>.Ok(new ProjectDto(project.Id, project.Name, project.Description, project.CreatedAt));
    }
}
