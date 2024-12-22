namespace ChallengeIt.Infrastructure.Security.TokenGenerator;

public class JwtSettings
{
    public required string Audience { get; init; }
    public required string Issuer { get; init; }
    public required string Secret { get; init; }
    
    public required int RefreshTokenExpirationInMinutes { get; init; }
    public required int AccessTokenExpirationInMinutes { get; init; }
}