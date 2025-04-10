using System.ComponentModel.DataAnnotations.Schema;

namespace ChallengeIt.Domain.Entities;

[Table("challenges")]
public class Challenge : Entity<Guid>
{
    public Challenge()
    {
        Id = Guid.NewGuid();
    }
    
    [Column("user_id")]
    public long? UserId { get; set; }
    
    [Column("title")]
    public required string Title { get; set; }
    
    // [Column("description")]
    // public string Description { get; set; }
    
    [Column("bet_amount")]
    public decimal BetAmount { get; set; }
    
    [Column("start_date")]
    public DateTime StartDate { get; set; }
    
    [Column("end_date")]
    public DateTime EndDate { get; set; }
    
    [Column("missed_days_count")]
    public int MissedDaysCount { get; set; }
    
    [Column("max_allowed_missed_days")]
    public int MaxAllowedMissedDaysCount { get; set; }
    
    [Column("status")]
    public ChallengeStatus Status { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    public bool IsActive(DateTime currentDate) => Status == ChallengeStatus.Pending || 
                                                  currentDate.Date >= StartDate.Date ||
                                                  currentDate.Date <= EndDate.Date;


    public ICollection<CheckIn>? CheckIns { get; set; }
    public User? User { get; set; }
}

public enum ChallengeStatus
{
    New,
    Accepted,
    Rejected,
    Canceled,
    Pending,
    Completed,
    Failed,
}