using FluentValidation;

namespace ChallengeIt.Application.Features.Challenges.Commands.CreateChallenge;

public class CreateChallengeCommandValidator : AbstractValidator<CreateChallengeCommand>
{
    public CreateChallengeCommandValidator()
    {
        RuleFor(x => x.Title)
            .MinimumLength(2)
            .MaximumLength(10000);

        RuleFor(x => x.BetAmount)
            .GreaterThan(10)
            .LessThanOrEqualTo(100);
    }
}