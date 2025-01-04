using ChallengeIt.API.Contracts.Challenges;
using ChallengeIt.Domain.Entities;
using ErrorOr;
using MediatR;

namespace ChallengeIt.Application.Features.Challenges.Commands.CreateChallenge;

public record CreateChallengeCommand(
    string Title,
    DateTime StartDate,
    DateTime EndDate,
    decimal BetAmount,
    List<DateTime> Schedule) : IRequest<ErrorOr<CreateChallengeResponse>>
{
    public Challenge MapToEntity() => new Challenge {
        Title = Title,
        BetAmount = BetAmount,
        StartDate = StartDate,
        EndDate = EndDate
    };
}