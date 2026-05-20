using Microsoft.EntityFrameworkCore;
using ProjectTaskManagement.Application.Interfaces;
using ProjectTaskManagement.Domain.Entities;

namespace ProjectTaskManagement.Application.UnitTests.TestHelpers;

public class FakeDbContext : DbContext, IApplicationDbContext
{
    public FakeDbContext(DbContextOptions<FakeDbContext> options) : base(options) { }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    public static FakeDbContext Create()
    {
        var options = new DbContextOptionsBuilder<FakeDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new FakeDbContext(options);
    }
}
