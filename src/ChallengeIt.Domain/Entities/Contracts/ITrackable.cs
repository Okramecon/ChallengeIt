namespace ChallengeIt.Domain.Entities.Contracts;

public interface ITrackable
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}