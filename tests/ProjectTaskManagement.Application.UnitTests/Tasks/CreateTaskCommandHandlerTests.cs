using FluentAssertions;
using Moq;
using ProjectTaskManagement.Application.Features.Tasks.Commands.CreateTask;
using ProjectTaskManagement.Application.Interfaces;
using ProjectTaskManagement.Application.UnitTests.TestHelpers;
using ProjectTaskManagement.Domain.Entities;
using ProjectTaskManagement.Domain.Enums;
using ProjectTaskManagement.Domain.Exceptions;

namespace ProjectTaskManagement.Application.UnitTests.Tasks;

public class CreateTaskCommandHandlerTests : IDisposable
{
    private readonly FakeDbContext _context;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly Mock<ICacheService> _cacheMock;

    public CreateTaskCommandHandlerTests()
    {
        _context = FakeDbContext.Create();
        _currentUserMock = new Mock<ICurrentUserService>();
        _cacheMock = new Mock<ICacheService>();
        _currentUserMock.Setup(x => x.UserId).Returns("user-123");
    }

    private CreateTaskCommandHandler CreateHandler() =>
        new(_context, _currentUserMock.Object, _cacheMock.Object);

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccessWithTask()
    {
        var project = new Project { UserId = "user-123", Name = "P" };
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        var cmd = new CreateTaskCommand("Task Title", "Desc", null, TaskPriority.High, project.Id);
        var result = await CreateHandler().Handle(cmd, CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Data!.Title.Should().Be("Task Title");
        result.Data.Priority.Should().Be(TaskPriority.High);
        result.Data.Status.Should().Be(TaskItemStatus.Pending);
    }

    [Fact]
    public async Task Handle_NonExistentProject_ThrowsNotFoundException()
    {
        var cmd = new CreateTaskCommand("T", "D", null, TaskPriority.Low, Guid.NewGuid());
        await CreateHandler().Invoking(h => h.Handle(cmd, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ProjectOwnedByOtherUser_ThrowsForbiddenException()
    {
        var project = new Project { UserId = "other-user", Name = "P" };
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        var cmd = new CreateTaskCommand("T", "D", null, TaskPriority.Low, project.Id);
        await CreateHandler().Invoking(h => h.Handle(cmd, CancellationToken.None))
            .Should().ThrowAsync<ForbiddenException>();
    }

    public void Dispose() => _context.Dispose();
}
