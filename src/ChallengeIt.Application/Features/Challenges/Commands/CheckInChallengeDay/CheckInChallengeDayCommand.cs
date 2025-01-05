using ErrorOr;
using MediatR;

namespace ChallengeIt.Application.Features.Challenges.Commands.CheckInChallengeDay;

public record CheckInChallengeDayCommand(
    Guid? ChallengeId,
    Guid? CheckInId) : IRequest<ErrorOr<Success>>;