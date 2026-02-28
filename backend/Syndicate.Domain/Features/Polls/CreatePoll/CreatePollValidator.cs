using FluentValidation;

namespace Syndicate.Domain.Features.Polls.CreatePoll;

public sealed class CreatePollValidator : AbstractValidator<CreatePollCommand>
{
    public CreatePollValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty().WithMessage("Group ID is required.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Options)
            .NotEmpty().WithMessage("At least one option is required.")
            .Must(o => o.Count >= 2).WithMessage("A poll must have at least 2 options.");

        RuleForEach(x => x.Options)
            .NotEmpty().WithMessage("Option text cannot be empty.")
            .MaximumLength(200).WithMessage("Option text must not exceed 200 characters.");
    }
}
