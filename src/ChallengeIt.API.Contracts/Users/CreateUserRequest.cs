namespace ChallengeIt.API.Contracts.Users;

public record CreateUserRequest(
    string Username,
    string Password,
    string Email,
    string? FIrstName = null,
    string? LastName = null);