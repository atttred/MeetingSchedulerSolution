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
}