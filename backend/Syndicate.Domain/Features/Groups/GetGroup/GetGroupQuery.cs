using MediatR;
using Syndicate.Domain.Common;
using Syndicate.Domain.DTOs.Groups;

namespace Syndicate.Domain.Features.Groups.GetGroup;

public sealed record GetGroupQuery(Guid GroupId) : IRequest<Result<GroupResponse>>;
