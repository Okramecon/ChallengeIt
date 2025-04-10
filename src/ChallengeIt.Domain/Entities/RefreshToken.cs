using System.ComponentModel.DataAnnotations.Schema;

namespace ChallengeIt.Domain.Entities;

[Table("refreshtokens")]
public class RefreshToken: Entity<Guid>
{
    [Column("token")]
    public required string Token { get; set; }
    
    [Column("user_id")]
    public long UserId { get; set; }
    
    [Column("expires_at")]
    public DateTime ExpiresAt { get; set; }
}