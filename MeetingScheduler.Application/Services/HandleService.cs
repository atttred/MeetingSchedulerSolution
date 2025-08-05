using MeetingScheduler.Domain.Models;
using MeetingScheduler.Domain.Services;

namespace MeetingScheduler.Application.Services;

public class HandleService : IHandleService
{
    public (DateTime StartTime, DateTime EndTime)? FindEarliestTimeSlot(List<int> participantIds, int durationMinutes,
        DateTime earliestStart, DateTime latestEnd, List<Meeting> existingMeetings)
    {
        var relevantMeetings = existingMeetings
            .Where(m => m.ParticipantIds.Any(id => participantIds.Contains(id)))
            .OrderBy(m => m.StartTime)
            .ToList();

        var currentStart = earliestStart;
        var duration = TimeSpan.FromMinutes(durationMinutes);

        while (currentStart + duration <= latestEnd)
        {
            var currentEnd = currentStart + duration;

            // starts/end during another meeting; overlaps
            bool hasConflict = relevantMeetings.Any(m =>
                (currentStart >= m.StartTime && currentStart < m.EndTime) ||
                (currentEnd > m.StartTime && currentEnd <= m.EndTime) ||
                (currentStart <= m.StartTime && currentEnd >= m.EndTime));

            if (!hasConflict)
            {
                return (currentStart, currentEnd);
            }

            var nextPossibleStart = relevantMeetings
                .Where(m => m.StartTime > currentStart)
                .Select(m => m.StartTime)
                .DefaultIfEmpty(latestEnd)
                .Min();

            currentStart = nextPossibleStart;
        }

        return null;
    }
}