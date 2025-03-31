using ChallengeIt.Application.Security;
using ChallengeIt.Domain.Errors;
using Microsoft.AspNetCore.Http;

namespace ChallengeIt.Infrastructure.Security.CurrentUserProvider;

public class CurrentUserProvider(IHttpContextAccessor httpContextAccessor) : ICurrentUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public long GetUserId()
    {
        var userId = GetSingleClaimValue("id");
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException(ApplicationErrors.MissedRequiredClaims.Message);
        
        return long.Parse(userId);
    }

    public string? GetEmail() => GetSingleClaimValue("email");
    
    public string? GetUsername() => GetSingleClaimValue("unique_name");

    public Guid? GetRefreshTokenId()
    {
        string? tokenId = GetSingleClaimValue("ref_tkn_id");
        return string.IsNullOrEmpty(tokenId) ? null : Guid.Parse(tokenId);
    }

    private string? GetSingleClaimValue(string claimName) =>
        _httpContextAccessor.HttpContext?.User.FindFirst(claimName)?.Value;
}