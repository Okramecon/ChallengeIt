namespace ChallengeIt.Application.Features.Auth.Results;

public record LoginResult(string AccessToken, string RefreshToken);