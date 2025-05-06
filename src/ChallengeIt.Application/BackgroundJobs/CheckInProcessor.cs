using ChallengeIt.Application.Persistence;
using Microsoft.Extensions.Logging;

namespace ChallengeIt.Application.BackgroundJobs
{
    public class CheckInProcessor(
        ILogger<CheckInProcessor> logger,
        IUnitOfWork unitOfWork)
        //ICheckInsRepository checkInsRepository,
        //IChallengesRepository challengesRepository)
    {
        public async Task ProcessTimezoneCheckins(string timezoneId)
        {
                var today = DateTime.UtcNow;
                var todayLocalDay = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(today, timezoneId);

                // Here checkins contains only id and challengeId
                var checkIns = await unitOfWork.CheckIns.GetUncheckedCheckInsForDateAsync(timezoneId, todayLocalDay);
                int missedChallengeActivityCount = 0;
            
                foreach (var checkIn in checkIns)
                {
                    logger.LogInformation($"Processing check-in with Id {checkIn.Id} in timezone {timezoneId}");

                    await unitOfWork.Challenges.ProcessMissedChallengeActivityAsync(checkIn.ChallengeId);
                missedChallengeActivityCount++;
                }

                unitOfWork.Commit();

            logger.LogInformation("Processing finished. Missed chellenges activities count: {MissedChallengeActivityCount} | TimeZone: {TimeZone}", missedChallengeActivityCount, timezoneId);
        }

        public async Task TryProcessTimezoneCheckins(string timezoneId)
        {
            try
            {
                await ProcessTimezoneCheckins(timezoneId);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                logger.LogError(ex, "Error processing check-ins for timezone {TimeZone}", timezoneId);
            }
        }
    }
}
 