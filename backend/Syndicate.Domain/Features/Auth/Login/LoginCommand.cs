using MediatR;
using Syndicate.Domain.Common;
using Syndicate.Domain.DTOs.Auth;

namespace Syndicate.Domain.Features.Auth.Login;

public sealed record LoginCommand(
    string Email,
    string Password) : IRequest<Result<AuthResponse>>;
