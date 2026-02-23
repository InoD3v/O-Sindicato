using MediatR;
using Syndicate.Domain.Common;
using Syndicate.Domain.DTOs.Debts;

namespace Syndicate.Domain.Features.Debts.GetGroupDebts;

public sealed record GetGroupDebtsQuery(
    Guid GroupId,
    Guid UserId) : IRequest<Result<List<DebtResponse>>>;
