using ErrorOr;
using MediatR;

namespace ChallengeIt.Application.Features.Challenges.Commands.UpdateChallenge;

public record UpdateChallengeCommand(Guid Id, string Title) : IRequest<ErrorOr<Success>>;