namespace ChallengeIt.Application.Security;

public interface ITokenProvider
{
    string GenerateJwtToken(long id, string email, string username);
    string GenerateRefreshToken();
}