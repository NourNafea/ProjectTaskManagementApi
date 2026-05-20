using FluentAssertions;
using Moq;
using ProjectTaskManagement.Application.Features.Projects.Commands.DeleteProject;
using ProjectTaskManagement.Application.Interfaces;
using ProjectTaskManagement.Application.UnitTests.TestHelpers;
using ProjectTaskManagement.Domain.Entities;
using ProjectTaskManagement.Domain.Exceptions;

namespace ProjectTaskManagement.Application.UnitTests.Projects;

public class DeleteProjectCommandHandlerTests : IDisposable
{
    private readonly FakeDbContext _context;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly Mock<ICacheService> _cacheMock;

    public DeleteProjectCommandHandlerTests()
    {
        _context = FakeDbContext.Create();
        _currentUserMock = new Mock<ICurrentUserService>();
        _cacheMock = new Mock<ICacheService>();
    }

    private DeleteProjectCommandHandler CreateHandler() =>
        new(_context, _currentUserMock.Object, _cacheMock.Object);

    [Fact]
    public async Task Handle_NonExistentProject_ThrowsNotFoundException()
    {
        var handler = CreateHandler();
        await handler.Invoking(h => h.Handle(new DeleteProjectCommand(Guid.NewGuid()), CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ProjectBelongingToOtherUser_ThrowsForbiddenException()
    {
        var project = new Project { Id = Guid.NewGuid(), UserId = "other-user", Name = "P" };
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        _currentUserMock.Setup(x => x.UserId).Returns("current-user");

        var handler = CreateHandler();
        await handler.Invoking(h => h.Handle(new DeleteProjectCommand(project.Id), CancellationToken.None))
            .Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Handle_OwnProject_ReturnsSuccessAndRemovesFromDb()
    {
        var project = new Project { Id = Guid.NewGuid(), UserId = "user-123", Name = "P" };
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        _currentUserMock.Setup(x => x.UserId).Returns("user-123");

        var handler = CreateHandler();
        var result = await handler.Handle(new DeleteProjectCommand(project.Id), CancellationToken.None);

        result.Success.Should().BeTrue();
        _context.Projects.Should().BeEmpty();
    }

    public void Dispose() => _context.Dispose();
}
