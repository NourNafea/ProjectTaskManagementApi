using FluentAssertions;
using Moq;
using ProjectTaskManagement.Application.Features.Auth.Commands.Register;
using ProjectTaskManagement.Application.Interfaces;

namespace ProjectTaskManagement.Application.UnitTests.Auth;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IIdentityService> _identityMock;
    private readonly Mock<IJwtTokenService> _jwtMock;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _identityMock = new Mock<IIdentityService>();
        _jwtMock = new Mock<IJwtTokenService>();
        _handler = new RegisterCommandHandler(_identityMock.Object, _jwtMock.Object);
    }

    [Fact]
    public async Task Handle_SuccessfulRegistration_ReturnsTokenAndUserInfo()
    {
        IList<string> roles = ["User"];
        _identityMock.Setup(x => x.RegisterAsync("Nour", "nour@test.com", "Pass123"))
            .ReturnsAsync((true, Array.Empty<string>(), "uid-1", "nour@test.com", "Nour", roles));
        _jwtMock.Setup(x => x.GenerateToken("uid-1", "nour@test.com", "Nour", roles))
            .Returns("jwt-token");

        var result = await _handler.Handle(new RegisterCommand("Nour", "nour@test.com", "Pass123"), CancellationToken.None);

        result.Success.Should().BeTrue();
        result.Data!.Token.Should().Be("jwt-token");
        result.Data.Email.Should().Be("nour@test.com");
        result.Data.FullName.Should().Be("Nour");
        result.Data.Roles.Should().Contain("User");
    }

    [Fact]
    public async Task Handle_FailedRegistration_ReturnsFailureWithErrors()
    {
        IList<string> emptyRoles = [];
        _identityMock.Setup(x => x.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((false, new[] { "Email already taken." }, string.Empty, string.Empty, string.Empty, emptyRoles));

        var result = await _handler.Handle(new RegisterCommand("N", "existing@test.com", "Pass"), CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Errors.Should().Contain("Email already taken.");
    }

    [Fact]
    public async Task Handle_FailedRegistration_DoesNotCallJwtService()
    {
        IList<string> emptyRoles = [];
        _identityMock.Setup(x => x.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((false, new[] { "Error" }, string.Empty, string.Empty, string.Empty, emptyRoles));

        await _handler.Handle(new RegisterCommand("N", "e@e.com", "p"), CancellationToken.None);

        _jwtMock.Verify(x => x.GenerateToken(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IList<string>>()), Times.Never);
    }
}
