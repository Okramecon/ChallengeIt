using ChallengeIt.Application.Persistence;
using ChallengeIt.Domain.Models.User;
using ErrorOr;
using MediatR;

namespace ChallengeIt.Application.Features.Users.Queries.FindUserProfile;

public class FindUserProfileQueryHandler(IUsersRepository usersRepository)
    : IRequestHandler<FindUserProfileQuery, ErrorOr<List<SearchUserProfileModel>>>
{
    private readonly IUsersRepository _usersRepository = usersRepository;

    public async Task<ErrorOr<List<SearchUserProfileModel>>> Handle(FindUserProfileQuery request, CancellationToken cancellationToken)
    {
        var userProfiles = await _usersRepository.FindUsersByNameAsync(request.SearchString, cancellationToken);

        return userProfiles;
    }
}