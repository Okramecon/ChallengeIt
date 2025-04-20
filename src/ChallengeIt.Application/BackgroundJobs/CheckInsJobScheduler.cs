using Hangfire;

namespace ChallengeIt.Application.BackgroundJobs
{
    public class CheckInsJobScheduler(IRecurringJobManager recurringJobManager)
    {
        private readonly IRecurringJobManager _recurringJobManager = recurringJobManager;

        public void ShceduleJobsForAllTimeZones()
        {
            var timeZones = TimeZoneInfo.GetSystemTimeZones();
            foreach (var timeZone in timeZones)
            {
                _recurringJobManager.AddOrUpdate<CheckInProcessor>(
                    $"checkin-processor-{timeZone.Id}",
                    processor => processor.ProcessTimezoneCheckins(timeZone.Id),
                    Cron.Daily(0, 5), // 12:05 AM
                    new RecurringJobOptions
                    {
                        TimeZone = timeZone
                    });
            }
        }
    }
}
