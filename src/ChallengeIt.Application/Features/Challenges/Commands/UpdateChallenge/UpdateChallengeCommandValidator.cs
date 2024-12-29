using FluentValidation;

namespace ChallengeIt.Application.Features.Challenges.Commands.UpdateChallenge;

public class UpdateChallengeCommandValidator : AbstractValidator<UpdateChallengeCommand>
{
    public UpdateChallengeCommandValidator()
    {
        RuleFor(x => x.Title)
            .MinimumLength(2)
            .MaximumLength(10000);
    }
}