namespace ChallengeIt.API.Contracts.Users;

public record GetUserInfoResponse(
    long Id,
    string Username, 
    string Email,
    string? FirstName,
    string? LastName);