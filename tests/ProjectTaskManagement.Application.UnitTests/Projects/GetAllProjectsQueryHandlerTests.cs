using FluentAssertions;
using Moq;
using ProjectTaskManagement.Application.Features.Projects.Queries.GetAllProjects;
using ProjectTaskManagement.Application.Interfaces;
using ProjectTaskManagement.Application.UnitTests.TestHelpers;
using ProjectTaskManagement.Domain.Entities;

namespace ProjectTaskManagement.Application.UnitTests.Projects;

public class GetAllProjectsQueryHandlerTests : IDisposable
{
    private readonly FakeDbContext _context;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly Mock<ICacheService> _cacheMock;

    public GetAllProjectsQueryHandlerTests()
    {
        _context = FakeDbContext.Create();
        _currentUserMock = new Mock<ICurrentUserService>();
        _cacheMock = new Mock<ICacheService>();
        _currentUserMock.Setup(x => x.UserId).Returns("user-123");
    }

    private GetAllProjectsQueryHandler CreateHandler() =>
        new(_context, _currentUserMock.Object, _cacheMock.Object);

    [Fact]
    public async Task Handle_ReturnsOnlyCurrentUserProjects()
    {
        _context.Projects.AddRange(
            new Project { UserId = "user-123", Name = "Mine" },
            new Project { UserId = "other-user", Name = "NotMine" });
        await _context.SaveChangesAsync();

        var result = await CreateHandler().Handle(new GetAllProjectsQuery(), CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(1);
        result.Data!.First().Name.Should().Be("Mine");
    }

    [Fact]
    public async Task Handle_NoProjects_ReturnsEmptyList()
    {
        var result = await CreateHandler().Handle(new GetAllProjectsQuery(), CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Data.Should().BeEmpty();
    }

    public void Dispose() => _context.Dispose();
}
