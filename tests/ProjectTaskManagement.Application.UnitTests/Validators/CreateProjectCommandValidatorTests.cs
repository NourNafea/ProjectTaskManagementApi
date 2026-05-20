using FluentAssertions;
using ProjectTaskManagement.Application.Features.Projects.Commands.CreateProject;

namespace ProjectTaskManagement.Application.UnitTests.Validators;

public class CreateProjectCommandValidatorTests
{
    private readonly CreateProjectCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        var result = _validator.Validate(new CreateProjectCommand("Valid Name", "Description"));
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyName_FailsValidation()
    {
        var result = _validator.Validate(new CreateProjectCommand("", "Description"));
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_NameTooLong_FailsValidation()
    {
        var result = _validator.Validate(new CreateProjectCommand(new string('A', 201), "Desc"));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_DescriptionTooLong_FailsValidation()
    {
        var result = _validator.Validate(new CreateProjectCommand("Name", new string('X', 1001)));
        result.IsValid.Should().BeFalse();
    }
}
