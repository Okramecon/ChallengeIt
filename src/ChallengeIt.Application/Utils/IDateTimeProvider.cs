namespace ChallengeIt.Application.Utils;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }

    DateOnly UtcDateOnlyNow => DateOnly.FromDateTime(UtcNow);
}