using MediatR;
using Syndicate.Domain.Common;
using Syndicate.Domain.DTOs.Auth;
using Syndicate.Domain.Interfaces;

namespace Syndicate.Domain.Features.Auth.Login;

public sealed class LoginHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            return Result.Failure<AuthResponse>("Invalid email or password.");

        var token = _jwtTokenService.GenerateToken(user.Id, user.Username, user.Email);

        return Result.Success(new AuthResponse(user.Id, user.Username, user.Email, token));
    }
}
