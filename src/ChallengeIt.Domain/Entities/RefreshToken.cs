namespace ChallengeIt.Domain.Entities;

public class RefreshToken: Entity<Guid>
{
    public required string Token { get; set; }
    public ulong UserId { get; set; }
    public DateTime ExpiresOnUtc { get; set; }
}