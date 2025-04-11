using System.ComponentModel.DataAnnotations.Schema;

namespace ChallengeIt.Domain.Entities;

[Table("checkins")]
public class CheckIn : Entity<Guid>
{
    public CheckIn()
    {
        Id = Guid.NewGuid();
    }
    
    [Column("date")]
    public DateTime Date { get; set; }
    
    [Column("user_id")]
    public long UserId { get; set; }
    
    [Column("challenge_id")]
    public Guid ChallengeId { get; set; }
    
    [Column("checked")]
    public bool Checked { get; set; }

    [Column("notes")]
    public string? Notes { get; set; }

    [Column("is_last")]
    public bool IsLast { get; set; }

    public User? User { get; set; }
    public Challenge? Challenge { get; set; }
}