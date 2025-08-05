using MeetingScheduler.API.Controllers;
using MeetingScheduler.API.DTOs;
using MeetingScheduler.Application.Services;
using MeetingScheduler.Domain.Services;
using MeetingScheduler.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace MeetingScheduler.Tests;

public class IntegrationTests
{
    private readonly MemoryRepository _repository;
    private readonly IHandleService _meetingService;
    private readonly MeetingsController _controller;

    public IntegrationTests()
    {
        _repository = new MemoryRepository();
        _meetingService = new HandleService();
        _controller = new MeetingsController(_meetingService, _repository);
    }

    private async Task ResetRepositoryAsync()
    {
        await _repository.ResetAsync();
    }

    [Fact]
    public async Task CreateUser_ReturnsCreated()
    {
        // Arrange
        await ResetRepositoryAsync();
        var request = new CreateUserRequest("Test User");

        // Act
        var result = await _controller.CreateUser(request);

        // Assert
        var createdResult = Assert.IsType<CreatedResult>(result);
        Assert.Equal(201, createdResult.StatusCode);
        var user = Assert.IsType<Domain.Models.User>(createdResult.Value);
        Assert.Equal("Test User", user.Name);
        Assert.True(user.Id > 0, "User ID should be assigned and greater than 0");
    }

    [Fact]
    public async Task CreateMeeting_NoAvailableSlot_ReturnsNotFound()
    {
        // Arrange
        await ResetRepositoryAsync();

        // Create users
        await _controller.CreateUser(new CreateUserRequest("Alice"));
        await _controller.CreateUser(new CreateUserRequest("Bob"));

        // full day meeting - Alice
        var fullDayMeeting = new CreateMeetingRequest(
            ParticipantIds: [1],
            DurationMinutes: 480,
            EarliestStart: new DateTime(2025, 6, 20, 9, 0, 0, DateTimeKind.Utc),
            LatestEnd: new DateTime(2025, 6, 20, 17, 0, 0, DateTimeKind.Utc)
        );
        var firstMeetingResult = await _controller.CreateMeeting(fullDayMeeting);
        Assert.IsType<CreatedResult>(firstMeetingResult); // intermediate check

        var newMeeting = new CreateMeetingRequest(
            ParticipantIds: [1, 2],
            DurationMinutes: 60,
            EarliestStart: new DateTime(2025, 6, 20, 9, 0, 0, DateTimeKind.Utc),
            LatestEnd: new DateTime(2025, 6, 20, 17, 0, 0, DateTimeKind.Utc)
        );

        // Act
        var result = await _controller.CreateMeeting(newMeeting);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(404, notFoundResult.StatusCode);
        Assert.Equal("No available time slot found", notFoundResult.Value);
    }

    [Fact]
    public async Task GetUserMeetings_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        await ResetRepositoryAsync();

        // Act
        var result = await _controller.GetUserMeetings(1);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(404, notFoundResult.StatusCode);
        Assert.Equal("User not found", notFoundResult.Value);
    }

    [Fact]
    public async Task GetUserMeetings_ReturnsMeetings()
    {
        // Arrange
        await ResetRepositoryAsync();
        var userResult = await _controller.CreateUser(new CreateUserRequest("Alice"));
        var user = Assert.IsType<Domain.Models.User>(((CreatedResult)userResult).Value);

        var meetingRequest = new CreateMeetingRequest(
            ParticipantIds: [user.Id],
            DurationMinutes: 60,
            EarliestStart: new DateTime(2025, 6, 20, 10, 0, 0, DateTimeKind.Utc),
            LatestEnd: new DateTime(2025, 6, 20, 11, 0, 0, DateTimeKind.Utc)
        );
        await _controller.CreateMeeting(meetingRequest);

        // Act
        var result = await _controller.GetUserMeetings(user.Id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        var meetings = Assert.IsAssignableFrom<IEnumerable<MeetingResponse>>(okResult.Value);
        Assert.Single(meetings);
        var meeting = meetings.First();
        Assert.True(meeting.Id > 0, "Meeting should have a valid ID");
        Assert.Contains(user.Id, meeting.ParticipantIds);
    }
}