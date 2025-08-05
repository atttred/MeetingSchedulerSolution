using MeetingScheduler.Domain.Models;

namespace MeetingScheduler.Domain.Services;

public interface IHandleService
{
    (DateTime StartTime, DateTime EndTime)? FindEarliestTimeSlot(List<int> participantIds, int durationMinutes,
        DateTime earliestStart, DateTime latestEnd, List<Meeting> existingMeetings);
}