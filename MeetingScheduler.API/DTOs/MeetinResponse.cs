namespace MeetingScheduler.API.DTOs;

public record MeetingResponse(int Id, List<int> ParticipantIds, DateTime StartTime, DateTime EndTime);