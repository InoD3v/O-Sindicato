using FluentValidation;

namespace Syndicate.Domain.Features.Groups.JoinGroup;

public sealed class JoinGroupValidator : AbstractValidator<JoinGroupCommand>
{
    public JoinGroupValidator()
    {
        RuleFor(x => x.InviteCode)
            .NotEmpty().WithMessage("Invite code is required.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");
    }
}
