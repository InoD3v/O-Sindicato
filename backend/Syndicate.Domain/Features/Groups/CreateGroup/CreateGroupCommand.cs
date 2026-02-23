using MediatR;
using Syndicate.Domain.Common;
using Syndicate.Domain.DTOs.Groups;

namespace Syndicate.Domain.Features.Groups.CreateGroup;

public sealed record CreateGroupCommand(
    string Name,
    string? Description,
    Guid UserId) : IRequest<Result<GroupResponse>>;
