using ChallengeIt.Application.Utils;

namespace ChallengeIt.Infrastructure.Utils;

public class DateTImeProvider : IDateTimeProvider 
{
    public DateTime UtcNow => DateTime.UtcNow;
}