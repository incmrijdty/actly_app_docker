using Microsoft.AspNetCore.Mvc;
using Actly.API.Models;
using Actly.API;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Actly.API.DTO;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]

public class EventsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public EventsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
    {
        return await _context.Events
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EventDto>> GetEvent(int id)
    {
        var ev = await _context.Events

            .Include(e => e.Organizer)
            .Include(e => e.Participations)
                .ThenInclude(p => p.User) 
            .FirstOrDefaultAsync(e => e.Id == id);

        if (ev == null)
            return NotFound();

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var dto = new EventDto
        {
            Id = ev.Id,
            Title = ev.Title,
            Description = ev.Description,
            Location = ev.Location,
            Category = ev.Category,
            MaxParticipants = ev.MaxParticipants,
            Date = ev.Date,
            Organizer = new UserDto
            {
                Id = ev.Organizer.Id,
                Username = ev.Organizer.Username,
                Email = ev.Organizer.Email,
                Role = ev.Organizer.Role
            },
            Participants = ev.Participations.Select(p => new ParticipationDto
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
                }
            }).ToList()
        };
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        return dto;
    }

    [Authorize(Roles = "Organizer")]
    [HttpGet("organizer/{organizerId}")]
    public async Task<IActionResult> GetByOrganizer(int organizerId)
    {
        var events = await _context.Events.Where(e => e.OrganizerId == organizerId).ToListAsync();
        return Ok(events);
    }


    [Authorize(Roles = "Organizer")]
    [HttpPost]

    public async Task<ActionResult<Event>> CreateEvent([FromBody] CreateEventDto dto)
    {

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized();

        var ev = new Event
        {
            Title = dto.Title,
            Description = dto.Description,
            Date = dto.Date.ToUniversalTime(),
            OrganizerId = int.Parse(userIdClaim.Value),
            Location = dto.Location,
            MaxParticipants = dto.MaxParticipants,
            Category = dto.Category,
            Participations = new List<Participation>()

        };

        _context.Events.Add(ev);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEvent), new { id = ev.Id }, ev);
    }

    [Authorize(Roles = "Organizer")]
    [HttpPut("{id}")]

    public async Task<IActionResult> UpdateEvent(int id, [FromBody] CreateEventDto dto)
    {
        var existing = await _context.Events.FirstOrDefaultAsync(e => e.Id == id);

        if (existing == null)
            return NotFound();

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || existing.OrganizerId != int.Parse(userIdClaim.Value))
            return Forbid();

        existing.Title = dto.Title;
        existing.Description = dto.Description;
        existing.Location = dto.Location;
        existing.Date = dto.Date.ToUniversalTime();
        existing.MaxParticipants = dto.MaxParticipants;
        existing.Category = dto.Category;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize(Roles = "Organizer")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        var ev = await _context.Events.FindAsync(id);
        if (ev == null)
            return NotFound();

        _context.Events.Remove(ev);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    
    [Authorize(Roles = "Organizer")]
    [HttpGet("{id}/participants")]
    public async Task<IActionResult> GetEventParticipants(int id)
    {
        var eventEntity = await _context.Events
            .Include(e => e.Participations)
            .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (eventEntity == null)
            return NotFound("Event not found.");

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        if (eventEntity.OrganizerId != userId)
            return Forbid("You're not the organizer of this event.");

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var participants = eventEntity.Participations.Select(p => new
        {
            p.User.Id,
            p.User.Username,
            p.Attended
        });
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        return Ok(participants);
    }

}
