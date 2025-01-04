using ChallengeIt.Application.Utils;
using FluentValidation;

namespace ChallengeIt.Application.Features.Challenges.Commands.CreateChallenge;

public class CreateChallengeCommandValidator : AbstractValidator<CreateChallengeCommand>
{
    public CreateChallengeCommandValidator(IDateTimeProvider dateTimeProvider)
    {
        RuleFor(x => x.Title)
            .MinimumLength(2)
            .MaximumLength(10000);

        RuleFor(x => x.BetAmount)
            .GreaterThan(10)
            .LessThanOrEqualTo(100);
        
        RuleFor(x => x.Schedule)
            .NotNull()
            .NotEmpty().WithMessage("You should provide a schedule dates.")
            .ForEach(date =>
            {
                date.NotEmpty().WithMessage("Date cannot be empty.")
                    .Must(d => d.Date >= dateTimeProvider.UtcNow.Date).WithMessage("Date must be in the future.");
            });
    }
}