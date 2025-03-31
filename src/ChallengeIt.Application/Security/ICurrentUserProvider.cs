namespace ChallengeIt.Application.Security;

public interface ICurrentUserProvider
{
    long GetUserId();
    string? GetUsername();
    string? GetEmail();
    Guid? GetRefreshTokenId();
}