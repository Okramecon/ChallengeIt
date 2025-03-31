using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ChallengeIt.Application.Security;
using ChallengeIt.Application.Utils;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ChallengeIt.Infrastructure.Security.TokenGenerator;

public class TokenProvider(IOptions<JwtSettings> jwtSettings, IDateTimeProvider dateTimeProvider): ITokenProvider
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    
    public string GenerateJwtToken(long id, string email, string username, Guid? refreshToken = null)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.UniqueName, username),
            new(JwtRegisteredClaimNames.Email, email),
            new("id", id.ToString()),
        };
        if (refreshToken.HasValue)
            claims.Add(new("ref_tkn_id", refreshToken.Value.ToString()));

        return GenerateJwtToken(claims);
    }

    public string GenerateJwtToken(IEnumerable<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            claims: claims,
            expires: _dateTimeProvider.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationInMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public (string Token, DateTime ExpiresAt) GenerateRefreshToken()
    {
        var expiresAt = _dateTimeProvider.UtcNow.AddMinutes(_jwtSettings.RefreshTokenExpirationInMinutes);
        return (Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)), expiresAt);
    }
}