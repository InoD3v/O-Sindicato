using MediatR;
using Syndicate.Domain.Common;
using Syndicate.Domain.DTOs.Auth;
using Syndicate.Domain.Entities;
using Syndicate.Domain.Interfaces;

namespace Syndicate.Domain.Features.Auth.Register;

public sealed class RegisterHandler : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.ExistsByEmailAsync(request.Email))
            return Result.Failure<AuthResponse>("Email is already in use.");

        if (await _userRepository.ExistsByUsernameAsync(request.Username))
            return Result.Failure<AuthResponse>("Username is already taken.");

        var hash = _passwordHasher.Hash(request.Password);
        var user = User.Create(request.Username, request.Email, hash);

        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var token = _jwtTokenService.GenerateToken(user.Id, user.Username, user.Email);

        return Result.Success(new AuthResponse(user.Id, user.Username, user.Email, token));
    }
}
