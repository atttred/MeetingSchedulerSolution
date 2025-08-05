using MeetingScheduler.API.DTOs;
using MeetingScheduler.Application.Services;
using MeetingScheduler.Domain.Services;
using MeetingScheduler.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class MeetingsController : ControllerBase
{
    private readonly IHandleService _meetingService;
    private readonly MemoryRepository _repository;

    public MeetingsController(IHandleService meetingService, MemoryRepository repository)
    {
        _meetingService = meetingService;
        _repository = repository;
    }

    [HttpPost("users")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var user = await _repository.CreateUserAsync(request.Name);
        return Created($"/api/users/{user.Id}", user);
    }

    [HttpPost("meetings")]
    public async Task<IActionResult> CreateMeeting([FromBody] CreateMeetingRequest request)
    {
        // validation of Ids
        foreach (var id in request.ParticipantIds)
        {
            if (!await _repository.UserExistsAsync(id))
            {
                return BadRequest("Invalid participant ID");
            }
        }

        // business hours
        var businessStart = request.EarliestStart.Date.AddHours(9);
        var businessEnd = request.EarliestStart.Date.AddHours(17);

        var earliestStart = request.EarliestStart < businessStart ? businessStart : request.EarliestStart;
        var latestEnd = request.LatestEnd > businessEnd ? businessEnd : request.LatestEnd;

        var slot = _meetingService.FindEarliestTimeSlot(request.ParticipantIds, request.DurationMinutes, earliestStart, latestEnd,
            await _repository.GetAllMeetingsAsync());

        if (slot == null)
        {
            return NotFound("No available time slot found");
        }

        var meeting = await _repository.CreateMeetingAsync(request.ParticipantIds, slot.Value.StartTime, slot.Value.EndTime);

        var response = new MeetingResponse(meeting.Id, meeting.ParticipantIds, meeting.StartTime, meeting.EndTime);

        return Created($"/api/meetings/{meeting.Id}", response);
    }

    [HttpGet("users/{userId}/meetings")]
    public async Task<IActionResult> GetUserMeetings(int userId)
    {
        // validation of Id
        if (!await _repository.UserExistsAsync(userId))
        {
            return NotFound("User not found");
        }

        var meetings = await _repository.GetUserMeetingsAsync(userId);
        var response = meetings
            .Select(m => new MeetingResponse(m.Id, m.ParticipantIds, m.StartTime, m.EndTime));

        return Ok(response);
    }
}