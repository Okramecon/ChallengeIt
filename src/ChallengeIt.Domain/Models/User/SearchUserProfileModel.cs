namespace ChallengeIt.Domain.Models.User;

public record SearchUserProfileModel(
    long Id,
    string Username,
    string FirstName,
    string LastName);