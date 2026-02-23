using FluentAssertions;
using NSubstitute;
using Syndicate.Domain.Entities;
using Syndicate.Domain.Features.Auth.Register;
using Syndicate.Domain.Interfaces;

namespace Syndicate.Tests.Domain.Features.Auth;

public class RegisterHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IJwtTokenService _jwtTokenService = Substitute.For<IJwtTokenService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly RegisterHandler _handler;

    public RegisterHandlerTests()
    {
        _handler = new RegisterHandler(_userRepository, _passwordHasher, _jwtTokenService, _unitOfWork);
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var command = new RegisterCommand("johndoe", "john@example.com", "password123");
        _userRepository.ExistsByEmailAsync(command.Email).Returns(false);
        _userRepository.ExistsByUsernameAsync(command.Username).Returns(false);
        _passwordHasher.Hash(command.Password).Returns("hashed_pw");
        _jwtTokenService.GenerateToken(Arg.Any<Guid>(), command.Username, command.Email)
            .Returns("jwt_token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Username.Should().Be("johndoe");
        result.Value.Email.Should().Be("john@example.com");
        result.Value.Token.Should().Be("jwt_token");
        await _userRepository.Received(1).AddAsync(Arg.Any<User>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithDuplicateEmail_ShouldReturnFailure()
    {
        var command = new RegisterCommand("johndoe", "existing@example.com", "password123");
        _userRepository.ExistsByEmailAsync(command.Email).Returns(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Email");
        await _userRepository.DidNotReceive().AddAsync(Arg.Any<User>());
    }

    [Fact]
    public async Task Handle_WithDuplicateUsername_ShouldReturnFailure()
    {
        var command = new RegisterCommand("existing", "john@example.com", "password123");
        _userRepository.ExistsByEmailAsync(command.Email).Returns(false);
        _userRepository.ExistsByUsernameAsync(command.Username).Returns(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Username");
    }
}
