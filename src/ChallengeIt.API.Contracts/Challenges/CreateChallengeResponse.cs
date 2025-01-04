namespace ChallengeIt.API.Contracts.Challenges;

public record CreateChallengeResponse(
    Guid Id,
    string Title,
    List<CreateCheckInModel> Schedule);