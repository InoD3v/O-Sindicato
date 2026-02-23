using MediatR;
using Syndicate.Domain.Common;
using Syndicate.Domain.DTOs.Members;

namespace Syndicate.Domain.Features.Members.GetBalance;

public sealed record GetBalanceQuery(
    Guid GroupId,
    Guid UserId) : IRequest<Result<BalanceResponse>>;
