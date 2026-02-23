using MediatR;
using Syndicate.Domain.Common;
using Syndicate.Domain.DTOs.Groups;

namespace Syndicate.Domain.Features.Groups.JoinGroup;

public sealed record JoinGroupCommand(
    string InviteCode,
    Guid UserId) : IRequest<Result<GroupResponse>>;
