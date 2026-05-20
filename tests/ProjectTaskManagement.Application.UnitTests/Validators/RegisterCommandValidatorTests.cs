using FluentAssertions;
using ProjectTaskManagement.Application.Features.Auth.Commands.Register;

namespace ProjectTaskManagement.Application.UnitTests.Validators;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        var result = _validator.Validate(new RegisterCommand("Full Name", "email@test.com", "Pass123"));
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_InvalidEmail_FailsValidation()
    {
        var result = _validator.Validate(new RegisterCommand("Name", "not-an-email", "Pass123"));
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact]
    public void Validate_ShortPassword_FailsValidation()
    {
        var result = _validator.Validate(new RegisterCommand("Name", "e@test.com", "123"));
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public void Validate_EmptyFullName_FailsValidation()
    {
        var result = _validator.Validate(new RegisterCommand("", "e@test.com", "Pass123"));
        result.IsValid.Should().BeFalse();
    }
}
