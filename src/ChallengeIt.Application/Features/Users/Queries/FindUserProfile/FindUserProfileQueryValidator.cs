using FluentValidation;

namespace ChallengeIt.Application.Features.Users.Queries.FindUserProfile;

public class FindUserProfileQueryValidator : AbstractValidator<FindUserProfileQuery>
{
    public FindUserProfileQueryValidator()
    {
        RuleFor(x => x.SearchString).NotEmpty().MaximumLength(50);
    }
}