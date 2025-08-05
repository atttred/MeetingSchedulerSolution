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

    public Task<User> CreateUserAsync(string name)
    {
        var user = new User
        {
            Id = _nextUserId++,
            Name = name
        };

        _users.TryAdd(user.Id, user);
        return Task.FromResult(user);
    }
    public Task<Meeting> CreateMeetingAsync(List<int> participantIds, DateTime startTime, DateTime endTime)
    {
        var meeting = new Meeting
        {
            Id = _nextMeetingId++,
            ParticipantIds = participantIds,
            StartTime = startTime,
            EndTime = endTime
        };

        _meetings.TryAdd(meeting.Id, meeting);
        return Task.FromResult(meeting);
    }

    public Task<List<Meeting>> GetUserMeetingsAsync(int userId)
    {
        return Task.FromResult(_meetings.Values
            .Where(m => m.ParticipantIds.Contains(userId))
            .ToList());
    }

    public Task<List<Meeting>> GetAllMeetingsAsync()
    {
        return Task.FromResult(_meetings.Values.ToList());
    }

    public Task<bool> UserExistsAsync(int userId)
    {
        return Task.FromResult(_users.ContainsKey(userId));
    }

    public Task ResetAsync()
    {
        _users.Clear();
        _meetings.Clear();
        _nextUserId = 1;
        _nextMeetingId = 1;
        return Task.CompletedTask;
    }
}