using FluentValidation;

namespace Syndicate.Domain.Features.Debts.CreateDebt;

public sealed class CreateDebtValidator : AbstractValidator<CreateDebtCommand>
{
    public CreateDebtValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty().WithMessage("Group ID is required.");

        RuleFor(x => x.CreditorUserId)
            .NotEmpty().WithMessage("Creditor user ID is required.");

        RuleFor(x => x.DebtorMemberId)
            .NotEmpty().WithMessage("Debtor member ID is required.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(300).WithMessage("Description must not exceed 300 characters.");
    }
}
