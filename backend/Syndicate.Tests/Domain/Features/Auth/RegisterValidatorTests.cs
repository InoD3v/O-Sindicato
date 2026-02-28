using FluentValidation.TestHelper;
using Syndicate.Domain.Features.Auth.Register;

namespace Syndicate.Tests.Domain.Features.Auth;

public class RegisterValidatorTests
{
    private readonly RegisterValidator _validator = new();

    [Fact]
    public void Validate_WithValidData_ShouldPass()
    {
        var command = new RegisterCommand("johndoe", "john@example.com", "password123");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("ab")]
    public void Validate_WithInvalidUsername_ShouldFail(string username)
    {
        var command = new RegisterCommand(username, "john@example.com", "password123");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void Validate_WithUsernameTooLong_ShouldFail()
    {
        var command = new RegisterCommand(new string('a', 31), "john@example.com", "password123");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public void Validate_WithInvalidEmail_ShouldFail(string email)
    {
        var command = new RegisterCommand("johndoe", email, "password123");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData("12345")]
    public void Validate_WithInvalidPassword_ShouldFail(string password)
    {
        var command = new RegisterCommand("johndoe", "john@example.com", password);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}
