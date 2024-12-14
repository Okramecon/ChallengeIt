using ChallengeIt.Domain.Entities.Contracts;

namespace ChallengeIt.Domain.Entities;

public class User(string username, string passwordHash, string email) : Entity<ulong>, ITrackable
{
    public string Username { get; set; } = username;
    public string Email { get; set; } = email;
    public string PasswordHash { get; set; } = passwordHash;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime CreateAt { get; set; }
    public DateTime? UpdateAt { get; set; }
}