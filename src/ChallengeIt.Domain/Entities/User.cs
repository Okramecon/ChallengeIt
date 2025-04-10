using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using ChallengeIt.Domain.Entities.Contracts;

namespace ChallengeIt.Domain.Entities;

[Table("users")]
public class User : Entity<long>, ITrackable
{
    [Column("username")]
    public required string Username { get; set; }
    
    [Column("email")]
    public required string Email { get; init; }
    
    [Column("password_hash")]
    public string? PasswordHash { get; init; }

    [Column("is_external")]
    public bool IsExternal { get; init; } = false;

    [Column("first_name")]
    public string? FirstName { get; init; }
    
    [Column("last_name")]
    public string? LastName { get; init; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Challenge> Challenges { get; set; } = [];
    public ICollection<CheckIn> CheckIns { get; set; } = [];
}