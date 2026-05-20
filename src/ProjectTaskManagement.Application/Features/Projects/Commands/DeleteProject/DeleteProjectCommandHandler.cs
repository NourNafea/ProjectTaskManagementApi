using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectTaskManagement.Application.Common;
using ProjectTaskManagement.Application.Interfaces;
using ProjectTaskManagement.Domain.Exceptions;

namespace ProjectTaskManagement.Application.Features.Projects.Commands.DeleteProject;

public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, ApiResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ICacheService _cache;

    public DeleteProjectCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, ICacheService cache)
    {
        _context = context;
        _currentUser = currentUser;
        _cache = cache;
    }

    public async Task<ApiResponse> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Project), request.Id);

        if (project.UserId != _currentUser.UserId)
            throw new ForbiddenException();

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync(cancellationToken);

        await Task.WhenAll(
            _cache.RemoveAsync($"projects:user:{_currentUser.UserId}", cancellationToken),
            _cache.RemoveAsync($"project:{_currentUser.UserId}:{request.Id}", cancellationToken));

        return ApiResponse.Ok("Project deleted.");
    }
}
