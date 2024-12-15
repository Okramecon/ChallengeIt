namespace ChallengeIt.API.Contracts.Auth;

public record LoginRequest(string Username, string Email, string Password);