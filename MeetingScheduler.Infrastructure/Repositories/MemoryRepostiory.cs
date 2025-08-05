using MeetingScheduler.Domain.Models;
using System.Collections.Concurrent;

namespace MeetingScheduler.Infrastructure.Repositories;

public class MemoryRepository
{
    // using concurrent for thread safety and atomic operations
    private readonly ConcurrentDictionary<int, Meeting> _meetings = new();
    private readonly ConcurrentDictionary<int, User> _users = new();
    private int _nextUserId = 1;
    private int _nextMeetingId = 1;

    public User CreateUser(string name)
    {
        var user = new User
        {
            Id = _nextUserId++,
            Name = name
        };

        _users.TryAdd(user.Id, user);
        return user;
    }
    public Meeting CreateMeeting(List<int> participantIds, DateTime startTime, DateTime endTime)
    {
        var meeting = new Meeting
        {
            Id = _nextMeetingId++,
            ParticipantIds = participantIds,
            StartTime = startTime,
            EndTime = endTime
        };

        _meetings.TryAdd(meeting.Id, meeting);
        return meeting;
    }

    public List<Meeting> GetUserMeetings(int userId)
    {
        return _meetings.Values
            .Where(m => m.ParticipantIds.Contains(userId))
            .ToList();
    }

    public List<Meeting> GetAllMeetings()
    {
        return _meetings.Values.ToList();
    }

    public bool UserExists(int userId)
    {
        return _users.ContainsKey(userId);
    }
}