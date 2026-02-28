using MediatR;
using Syndicate.Domain.Common;
using Syndicate.Domain.DTOs.Debts;
using Syndicate.Domain.Interfaces;

namespace Syndicate.Domain.Features.Debts.GetGroupDebts;

public sealed class GetGroupDebtsHandler : IRequestHandler<GetGroupDebtsQuery, Result<List<DebtResponse>>>
{
    private readonly IDebtRepository _debtRepository;
    private readonly IMemberRepository _memberRepository;

    public GetGroupDebtsHandler(IDebtRepository debtRepository, IMemberRepository memberRepository)
    {
        _debtRepository = debtRepository;
        _memberRepository = memberRepository;
    }

    public async Task<Result<List<DebtResponse>>> Handle(GetGroupDebtsQuery request, CancellationToken cancellationToken)
    {
        var member = await _memberRepository.GetByUserAndGroupAsync(request.UserId, request.GroupId);
        if (member is null)
            return Result.Failure<List<DebtResponse>>("You are not a member of this group.");

        var debts = await _debtRepository.GetByGroupIdAsync(request.GroupId);

        var response = debts.Select(d => new DebtResponse(
            d.Id,
            d.GroupId,
            d.CreditorMemberId,
            d.Creditor.User.Username,
            d.DebtorMemberId,
            d.Debtor.User.Username,
            d.Amount,
            d.Description,
            d.Status,
            d.CreatedAt)).ToList();

        return Result.Success(response);
    }
}
