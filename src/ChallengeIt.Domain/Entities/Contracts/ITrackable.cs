namespace ChallengeIt.Domain.Entities.Contracts;

public interface ITrackable
{
    public DateTime CreateAt { get; set; }
    public DateTime? UpdateAt { get; set; }
}