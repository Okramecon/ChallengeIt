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
    bool Everyday,
    string? Goal,
    string? Motivation,
    string? MinimalActivityDescription,
    int MinimalActivityMinutesTimer = 0,
    int MaxAllowedMissedDaysCount = 1,
    int ThemeCode = 0,
    bool IsPrivate = false,
    string TimeZone = "UTC",
    List<DateTime>? Schedule = null) : IRequest<ErrorOr<CreateChallengeResponse>>
{
    public Challenge MapToEntity() => new()
    {
        Title = Title,
        BetAmount = BetAmount,
        StartDate = StartDate,
        EndDate = EndDate,
        Goal = Goal ?? string.Empty,
        Motivation = Motivation,
        MinimalActivityDescription = MinimalActivityDescription,
        MinimalActivityMinutesTimer = MinimalActivityMinutesTimer,
        MaxAllowedMissedDaysCount = MaxAllowedMissedDaysCount,
        ThemeCode = ThemeCode,
        IsPrivate = IsPrivate,
    };
}