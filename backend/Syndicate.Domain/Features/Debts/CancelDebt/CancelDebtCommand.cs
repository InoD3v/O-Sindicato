using MediatR;
using Syndicate.Domain.Common;
using Syndicate.Domain.DTOs.Debts;

namespace Syndicate.Domain.Features.Debts.CancelDebt;

public sealed record CancelDebtCommand(
    Guid DebtId,
    Guid UserId) : IRequest<Result<DebtResponse>>;
