using FluentAssertions;
using Moq;
using ProjectTaskManagement.Application.Features.Auth.Commands.Login;
using ProjectTaskManagement.Application.Interfaces;

namespace ProjectTaskManagement.Application.UnitTests.Auth;

public class LoginCommandHandlerTests
{
    private readonly Mock<IIdentityService> _identityMock;
    private readonly Mock<IJwtTokenService> _jwtMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _identityMock = new Mock<IIdentityService>();
        _jwtMock = new Mock<IJwtTokenService>();
        _handler = new LoginCommandHandler(_identityMock.Object, _jwtMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsToken()
    {
        IList<string> roles = ["User"];
        _identityMock.Setup(x => x.LoginAsync("user@test.com", "Pass123"))
            .ReturnsAsync((true, "uid-1", "user@test.com", "User Name", roles));
        _jwtMock.Setup(x => x.GenerateToken("uid-1", "user@test.com", "User Name", roles))
            .Returns("valid-token");

        var result = await _handler.Handle(new LoginCommand("user@test.com", "Pass123"), CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Data!.Token.Should().Be("valid-token");
        result.Data.Roles.Should().Contain("User");
    }

    [Fact]
    public async Task Handle_InvalidCredentials_ReturnsFailure()
    {
        IList<string> emptyRoles = [];
        _identityMock.Setup(x => x.LoginAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((false, string.Empty, string.Empty, string.Empty, emptyRoles));

        var result = await _handler.Handle(new LoginCommand("bad@test.com", "wrong"), CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Invalid");
    }
}
