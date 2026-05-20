using FluentAssertions;
using Moq;
using ProjectTaskManagement.Application.Features.Projects.Commands.CreateProject;
using ProjectTaskManagement.Application.Interfaces;
using ProjectTaskManagement.Application.UnitTests.TestHelpers;

namespace ProjectTaskManagement.Application.UnitTests.Projects;

public class CreateProjectCommandHandlerTests : IDisposable
{
    private readonly FakeDbContext _context;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly Mock<ICacheService> _cacheMock;
    private readonly CreateProjectCommandHandler _handler;

    public CreateProjectCommandHandlerTests()
    {
        _context = FakeDbContext.Create();
        _currentUserMock = new Mock<ICurrentUserService>();
        _cacheMock = new Mock<ICacheService>();
        _currentUserMock.Setup(x => x.UserId).Returns("user-123");
        _handler = new CreateProjectCommandHandler(_context, _currentUserMock.Object, _cacheMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccessResponse()
    {
        var result = await _handler.Handle(new CreateProjectCommand("My Project", "Description"), CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("My Project");
        result.Data.Description.Should().Be("Description");
    }

    [Fact]
    public async Task Handle_ValidCommand_ProjectPersistedToDatabase()
    {
        await _handler.Handle(new CreateProjectCommand("Persisted", "Desc"), CancellationToken.None);

        _context.Projects.Should().HaveCount(1);
        _context.Projects.First().UserId.Should().Be("user-123");
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsProjectWithId()
    {
        var result = await _handler.Handle(new CreateProjectCommand("Test", "Desc"), CancellationToken.None);

        result.Data!.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_ValidCommand_InvalidatesCacheForUser()
    {
        await _handler.Handle(new CreateProjectCommand("Test", "Desc"), CancellationToken.None);

        _cacheMock.Verify(x => x.RemoveAsync($"projects:user:user-123", It.IsAny<CancellationToken>()), Times.Once);
    }

    public void Dispose() => _context.Dispose();
}
