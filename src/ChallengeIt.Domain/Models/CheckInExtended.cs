namespace ChallengeIt.Domain.Models
{
    public class TodayCheckInModel
    {
        public required Guid Id { get; init; }
        public required DateTime Date { get; init; }

        public required long UserId { get; init; }

        public required Guid ChallengeId { get; init; }

        public required bool Checked { get; init; }

        public string? ChallengeTitle { get; init; }
    }
}
