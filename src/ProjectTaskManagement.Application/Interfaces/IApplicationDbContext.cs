using Microsoft.EntityFrameworkCore;
using ProjectTaskManagement.Domain.Entities;

namespace ProjectTaskManagement.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Project> Projects { get; }
    DbSet<TaskItem> Tasks { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
