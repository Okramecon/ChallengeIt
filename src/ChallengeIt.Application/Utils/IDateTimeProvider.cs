namespace ChallengeIt.Application.Utils;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}