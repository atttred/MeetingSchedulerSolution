namespace MeetingScheduler.API.DTOs;

public record CreateMeetingRequest(List<int> ParticipantIds, int DurationMinutes, DateTime EarliestStart, DateTime LatestEnd);