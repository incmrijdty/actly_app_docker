using Microsoft.AspNetCore.Mvc;
using Actly.API.Models;
using Actly.API.DTO;
using Actly.API;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class ParticipationController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ParticipationController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Participation>>> GetParticipations()
    {
        return await _context.Participations
            .Include(p => p.User)
            .Include(p => p.Event)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ParticipationDto>> GetParticipation(int id)
    {
        var p = await _context.Participations
            .Include(p => p.User)
            .Include(p => p.Event)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (p == null)
            return NotFound();

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        return new ParticipationDto
        {
            Id = p.Id,
            UserId = p.UserId,
            EventId = p.EventId,
            Attended = p.Attended,
            User = new UserDto
            {
                Id = p.User.Id,
                Username = p.User.Username,
                Email = p.User.Email,
                Role = p.User.Role
            },
            Event = new EventBriefDto
            {
                Id = p.Event.Id,
                Title = p.Event.Title,
                Date = p.Event.Date
            }
        };
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    }

    [Authorize(Roles = "Volunteer")]
    [HttpPost]
    public async Task<ActionResult<Participation>> JoinEvent([FromBody] ParticipationDto dto)
    {
        var alreadyJoined = await _context.Participations
            .AnyAsync(p => p.UserId == dto.UserId && p.EventId == dto.EventId);

        if (alreadyJoined)
            return BadRequest("User already joined this event.");

        var user = await _context.Users.FindAsync(dto.UserId);
        var ev = await _context.Events.FindAsync(dto.EventId);

        if (user == null || ev == null)
            return BadRequest("Invalid user or event ID.");

        var participation = new Participation
        {
            UserId = dto.UserId,
            EventId = dto.EventId,
            Attended = dto.Attended,
            JoinedAt = DateTime.UtcNow,
            User = user,
            Event = ev
        };

        _context.Participations.Add(participation);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetParticipations), new { id = participation.Id }, participation);
    }

    [Authorize(Roles = "Volunteer")]
    [HttpDelete("{userId}/{eventId}")]
    public async Task<IActionResult> LeaveEvent(int userId, int eventId)
    {
        var part = await _context.Participations.FirstOrDefaultAsync(p => p.UserId == userId && p.EventId == eventId);
        if (part == null)
            return NotFound();

        _context.Participations.Remove(part);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("user/{userId}/event/{eventId}")]
    public async Task<IActionResult> CheckParticipation(int userId, int eventId)
    {
        var exists = await _context.Participations
            .AnyAsync(p => p.UserId == userId && p.EventId == eventId);

        return Ok(exists);
    }

    [HttpPut("{id}/attendance")]
    public async Task<IActionResult> UpdateAttendance(int id, [FromBody] bool attended)
    {
        var participation = await _context.Participations.FindAsync(id);
        if (participation == null) return NotFound();

        participation.Attended = attended;
        await _context.SaveChangesAsync();

        return NoContent();
    }


}
