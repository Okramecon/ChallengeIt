using System.ComponentModel.DataAnnotations.Schema;
using ChallengeIt.Domain.Entities.Contracts;

namespace ChallengeIt.Domain.Entities;

public class User : Entity<long>, ITrackable
{
    [Column("username")]
    public required string Username { get; set; }
    
    [Column("email")]
    public required string Email { get; init; }
    
    [Column("password_hash")]
    public required string PasswordHash { get; init; }
    
    [Column("first_name")]
    public string? FirstName { get; init; }
    
    [Column("last_name")]
    public string? LastName { get; init; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}