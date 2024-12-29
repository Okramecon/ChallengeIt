using ChallengeIt.Application.Persistence;
using ChallengeIt.Application.Security;
using ChallengeIt.Domain.Entities;
using ChallengeIt.Domain.Models.Paging;
using ErrorOr;
using MediatR;

namespace ChallengeIt.Application.Features.Challenges.Queries;

public class GetUserChallengesListQueryHandler(
    ICurrentUserProvider currentUserProvider,
    IChallengesRepository challengesRepository)
    : IRequestHandler<GetUserChallengesListQuery, ErrorOr<Page<Challenge>>>
{
    private readonly ICurrentUserProvider _currentUserProvider = currentUserProvider;
    private readonly IChallengesRepository _challengesRepository = challengesRepository;
    
    public async Task<ErrorOr<Page<Challenge>>> Handle(GetUserChallengesListQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserProvider.GetUserId();
        var pageRequest = new PageRequest(request.PageNumber, request.PageSize);
        
        var challengesPage = await _challengesRepository.GetUserChallengesAsync(pageRequest, userId, cancellationToken);

        return challengesPage;
    }
}