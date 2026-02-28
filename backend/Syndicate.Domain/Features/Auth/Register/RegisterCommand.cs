using MediatR;
using Syndicate.Domain.Common;
using Syndicate.Domain.DTOs.Auth;

namespace Syndicate.Domain.Features.Auth.Register;

public sealed record RegisterCommand(
    string Username,
    string Email,
    string Password) : IRequest<Result<AuthResponse>>;
