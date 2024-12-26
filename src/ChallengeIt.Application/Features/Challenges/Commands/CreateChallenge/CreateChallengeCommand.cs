using ChallengeIt.Domain.Entities;
using ErrorOr;
using MediatR;

namespace ChallengeIt.Application.Features.Challenges.Commands.CreateChallenge;

public record CreateChallengeCommand(
    string Title,
    DateTime StartDate,
    DateTime EndDate,
    decimal BetAmount) : IRequest<ErrorOr<Guid>>
{
    public Challenge MapToEntity() => new Challenge {
        Title = Title,
        BetAmount = BetAmount,
        StartDate = StartDate,
        EndDate = EndDate
    };
}