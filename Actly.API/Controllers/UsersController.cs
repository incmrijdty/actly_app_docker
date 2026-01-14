using Microsoft.AspNetCore.Mvc;
using Actly.API.Models;
using Actly.API;
using Actly.API.DTO;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UsersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var user = await _context.Users
        .Include(u => u.Participations)
            .ThenInclude(p => p.Event)
        .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return NotFound();

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        var dto = new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            Participations = user.Participations.Select(p => new ParticipationDto
            {
                Id = p.Id,
                UserId = p.UserId,
                EventId = p.EventId,
                Attended = p.Attended,
                Event = new EventBriefDto
                {
                    Id = p.Event.Id,
                    Title = p.Event.Title,
                    Date = p.Event.Date
                }
            }).ToList()
        };
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        return dto;
    }

    [HttpPost]
    public async Task<ActionResult<User>> CreateUser(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    [HttpPut("{id}")]

    public async Task<IActionResult> UpdateUser(int id, User updated)
    {
        if (id != updated.Id)
            return BadRequest();

        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound();

        user.Username = updated.Username;
        user.Email = updated.Email;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{id}/events")]
    public async Task<ActionResult<IEnumerable<EventDto>>> GetUserEvents(int id)
    {
        var events = await _context.Participations
            .Where(p => p.UserId == id && p.Event != null)
            .Include(p => p.Event)
            .Select(p => new EventDto
            {
                Id = p.Event!.Id,
                Title = p.Event.Title,
                Date = p.Event.Date,
                Description = p.Event.Description,
                Attended = p.Attended,
                Location = p.Event.Location,
                Category = p.Event.Category,
                MaxParticipants = p.Event.MaxParticipants
            })
            .ToListAsync();

        return Ok(events);
    }
    


}