using System.Security.Claims;

namespace ChallengeIt.Application.Security;

public interface ITokenProvider
{
    string GenerateJwtToken(long id, string email, string username, Guid? refreshToken = null);
    string GenerateJwtToken(IEnumerable<Claim> claims);
    (string Token, DateTime ExpiresAt) GenerateRefreshToken();
}