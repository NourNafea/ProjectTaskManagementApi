using FluentAssertions;
using Moq;
using ProjectTaskManagement.Application.Features.Tasks.Commands.UpdateTaskStatus;
using ProjectTaskManagement.Application.Interfaces;
using ProjectTaskManagement.Application.UnitTests.TestHelpers;
using ProjectTaskManagement.Domain.Entities;
using ProjectTaskManagement.Domain.Enums;
using ProjectTaskManagement.Domain.Exceptions;

namespace ProjectTaskManagement.Application.UnitTests.Tasks;

public class UpdateTaskStatusCommandHandlerTests : IDisposable
{
    private readonly FakeDbContext _context;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly Mock<ICacheService> _cacheMock;

    public UpdateTaskStatusCommandHandlerTests()
    {
        _context = FakeDbContext.Create();
        _currentUserMock = new Mock<ICurrentUserService>();
        _cacheMock = new Mock<ICacheService>();
        _currentUserMock.Setup(x => x.UserId).Returns("user-123");
    }

    private UpdateTaskStatusCommandHandler CreateHandler() =>
        new(_context, _currentUserMock.Object, _cacheMock.Object);

    private async Task<(Project project, TaskItem task)> SeedData(string userId = "user-123")
    {
        var project = new Project { UserId = userId, Name = "P" };
        var task = new TaskItem { Title = "T", ProjectId = project.Id, Project = project };
        _context.Projects.Add(project);
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return (project, task);
    }

    [Fact]
    public async Task Handle_ValidUpdate_ChangesStatus()
    {
        var (_, task) = await SeedData();

        var result = await CreateHandler().Handle(
            new UpdateTaskStatusCommand(task.Id, TaskItemStatus.Completed), CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Data!.Status.Should().Be(TaskItemStatus.Completed);
    }

    [Fact]
    public async Task Handle_NonExistentTask_ThrowsNotFoundException()
    {
        await CreateHandler().Invoking(h => h.Handle(
            new UpdateTaskStatusCommand(Guid.NewGuid(), TaskItemStatus.InProgress), CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_TaskOwnedByOtherUser_ThrowsForbiddenException()
    {
        var (_, task) = await SeedData("other-user");

        await CreateHandler().Invoking(h => h.Handle(
            new UpdateTaskStatusCommand(task.Id, TaskItemStatus.Completed), CancellationToken.None))
            .Should().ThrowAsync<ForbiddenException>();
    }

    public void Dispose() => _context.Dispose();
}
