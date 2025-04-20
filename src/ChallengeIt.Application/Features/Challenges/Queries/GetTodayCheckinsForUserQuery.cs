using ChallengeIt.Application.Persistence;
using ChallengeIt.Application.Security;
using ChallengeIt.Application.Utils;
using ChallengeIt.Domain.Models;
using ErrorOr;
using MediatR;

namespace ChallengeIt.Application.Features.Challenges.Queries
{
    public record struct GetTodayCheckinsForUserQuery : IRequest<ErrorOr<List<TodayCheckInModel>>>;

    public class GetTodayCheckInsForUserQueryHandler(
        ICurrentUserProvider currentUserProvider,
        IDateTimeProvider dateTimeProvider,
        ICheckInsRepository challengesRepository) : IRequestHandler<GetTodayCheckinsForUserQuery, ErrorOr<List<TodayCheckInModel>>>
    {
        private readonly ICurrentUserProvider _currentUserProvider = currentUserProvider;
        private readonly ICheckInsRepository _challengesRepository = challengesRepository;
        private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;

        public async Task<ErrorOr<List<TodayCheckInModel>>> Handle(GetTodayCheckinsForUserQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserProvider.GetUserId();

            var today = _dateTimeProvider.UtcNow.Date;

            var todayCheckIns = await _challengesRepository.GetCheckinsForDateAsync(today, userId, cancellationToken);
            return todayCheckIns;
        }
    }
}
