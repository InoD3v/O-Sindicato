using FluentAssertions;
using NSubstitute;
using Syndicate.Domain.Entities;
using Syndicate.Domain.Features.Auth.Login;
using Syndicate.Domain.Interfaces;

namespace Syndicate.Tests.Domain.Features.Auth;

public class LoginHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IJwtTokenService _jwtTokenService = Substitute.For<IJwtTokenService>();
    private readonly LoginHandler _handler;

    public LoginHandlerTests()
    {
        _handler = new LoginHandler(_userRepository, _passwordHasher, _jwtTokenService);
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ShouldReturnSuccess()
    {
        // Arrange
        var user = User.Create("johndoe", "john@example.com", "hashed_pw");
        var command = new LoginCommand("john@example.com", "password123");

        _userRepository.GetByEmailAsync(command.Email).Returns(user);
        _passwordHasher.Verify(command.Password, user.PasswordHash).Returns(true);
        _jwtTokenService.GenerateToken(user.Id, user.Username, user.Email)
            .Returns("jwt_token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Username.Should().Be("johndoe");
        result.Value.Token.Should().Be("jwt_token");
    }

    [Fact]
    public async Task Handle_WithNonExistentEmail_ShouldReturnFailure()
    {
        var command = new LoginCommand("unknown@example.com", "password123");
        _userRepository.GetByEmailAsync(command.Email).Returns((User?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Invalid email or password");
    }

    [Fact]
    public async Task Handle_WithWrongPassword_ShouldReturnFailure()
    {
        var user = User.Create("johndoe", "john@example.com", "hashed_pw");
        var command = new LoginCommand("john@example.com", "wrong_password");

        _userRepository.GetByEmailAsync(command.Email).Returns(user);
        _passwordHasher.Verify(command.Password, user.PasswordHash).Returns(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Invalid email or password");
    }
}
