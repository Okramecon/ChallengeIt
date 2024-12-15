namespace ChallengeIt.API.Contracts.Auth;

public record LoginResponse(string AccessToken, string RefreshToken);