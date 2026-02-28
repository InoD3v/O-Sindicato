using FluentValidation;

namespace Syndicate.Domain.Features.Polls.CastVote;

public sealed class CastVoteValidator : AbstractValidator<CastVoteCommand>
{
    public CastVoteValidator()
    {
        RuleFor(x => x.PollId)
            .NotEmpty().WithMessage("Poll ID is required.");

        RuleFor(x => x.PollOptionId)
            .NotEmpty().WithMessage("Poll option ID is required.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");
    }
}
