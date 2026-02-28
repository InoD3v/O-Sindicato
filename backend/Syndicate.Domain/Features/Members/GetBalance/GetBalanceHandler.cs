using MediatR;
using Syndicate.Domain.Common;
using Syndicate.Domain.DTOs.Members;
using Syndicate.Domain.Interfaces;

namespace Syndicate.Domain.Features.Members.GetBalance;

public sealed class GetBalanceHandler : IRequestHandler<GetBalanceQuery, Result<BalanceResponse>>
{
    private readonly IMemberRepository _memberRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IDebtRepository _debtRepository;

    public GetBalanceHandler(
        IMemberRepository memberRepository,
        ITransactionRepository transactionRepository,
        IDebtRepository debtRepository)
    {
        _memberRepository = memberRepository;
        _transactionRepository = transactionRepository;
        _debtRepository = debtRepository;
    }

    public async Task<Result<BalanceResponse>> Handle(GetBalanceQuery request, CancellationToken cancellationToken)
    {
        var member = await _memberRepository.GetByUserAndGroupAsync(request.UserId, request.GroupId);
        if (member is null)
            return Result.Failure<BalanceResponse>("You are not a member of this group.");

        var totalBalance = await _transactionRepository.GetBalanceAsync(member.Id);
        var blockedBalance = await _debtRepository.GetBlockedBalanceAsync(member.Id);
        var availableBalance = totalBalance - blockedBalance;

        return Result.Success(new BalanceResponse(
            member.Id,
            member.GroupId,
            totalBalance,
            blockedBalance,
            availableBalance));
    }
}
