using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Actly.API.Models;
using Actly.API.DTO;
using Actly.API;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]

public class AuthController : ControllerBase
{

    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(ApplicationDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return BadRequest("Email already in use.");


        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            Role = dto.Role,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Participations = []
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok("User registered successfully.");
    }



    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Unauthorized("Invalid credentials.");

        var token = GenerateJwtToken(user);
        return Ok(new { token });
    }

    private string GenerateJwtToken(User user)
    {
        var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY");
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var expiresValue = _config["Jwt:Expires"];
        if (string.IsNullOrEmpty(expiresValue))
            throw new Exception("Jwt:Expires is missing from configuration!");

#pragma warning disable CS8604 // Possible null reference argument.
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(expiresValue)),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)), SecurityAlgorithms.HmacSha256)
        );
#pragma warning restore CS8604 // Possible null reference argument.

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}