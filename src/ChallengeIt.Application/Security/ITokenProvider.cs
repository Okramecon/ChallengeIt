namespace ChallengeIt.Application.Security;

public interface ITokenProvider
{
    string GenerateJwtToken(ulong id, string email, string username);
    string GenerateRefreshToken();
}